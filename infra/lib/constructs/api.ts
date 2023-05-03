import {
  CorsHttpMethod,
  DomainName,
  HttpApi,
  HttpMethod,
  HttpNoneAuthorizer,
} from "@aws-cdk/aws-apigatewayv2-alpha";
import { HttpJwtAuthorizer } from "@aws-cdk/aws-apigatewayv2-authorizers-alpha";
import { HttpLambdaIntegration } from "@aws-cdk/aws-apigatewayv2-integrations-alpha";
import * as cdk from "aws-cdk-lib";
import { ICertificate } from "aws-cdk-lib/aws-certificatemanager";
import { IFunction } from "aws-cdk-lib/aws-lambda";
import { ARecord, IHostedZone, RecordTarget } from "aws-cdk-lib/aws-route53";
import { ApiGatewayv2DomainProperties } from "aws-cdk-lib/aws-route53-targets";
import { Construct } from "constructs";

export interface ApiProps {
  readonly userPoolId: string;
  readonly userPoolAppIntegrationClientIds: string[];
  readonly lambdaFunction: IFunction;
  readonly certificate: ICertificate;
  readonly hostedZone: IHostedZone;
  readonly domainName: string;
}

export class Api extends Construct {
  private readonly API_BASE_PATH = "/api/v1";

  constructor(scope: Construct, id: string, props: ApiProps) {
    super(scope, id);

    const issuerUrl = `https://cognito-idp.${
      cdk.Stack.of(this).region
    }.amazonaws.com/${props.userPoolId}`;

    const cognitoJwtAuthorizer = new HttpJwtAuthorizer(
      "CognitoJwtAuthorizer",
      issuerUrl,
      {
        jwtAudience: props.userPoolAppIntegrationClientIds,
      }
    );

    const domain = new DomainName(this, "DomainName", {
      certificate: props.certificate,
      domainName: props.domainName,
    });

    const httpApi = new HttpApi(this, "BeMyGuestHttpApi", {
      description: "Be my guest API gateway",
      corsPreflight: {
        allowHeaders: [
          "Content-Type",
          "X-Amz-Date",
          "Authorization",
          "X-Api-Key",
        ],
        allowMethods: [
          CorsHttpMethod.OPTIONS,
          CorsHttpMethod.GET,
          CorsHttpMethod.POST,
          CorsHttpMethod.PUT,
          CorsHttpMethod.PATCH,
          CorsHttpMethod.DELETE,
        ],
        allowCredentials: true,
        allowOrigins: ["http://localhost:3000"],
      },
      defaultAuthorizer: cognitoJwtAuthorizer,
      defaultDomainMapping: {
        domainName: domain,
      },
      disableExecuteApiEndpoint: true,
    });

    const integration = new HttpLambdaIntegration(
      "HttpLambdaIntegration",
      props.lambdaFunction
    );

    httpApi.addRoutes({
      path: "/",
      methods: [HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/calculator/add/{a}/{b}`,
      methods: [HttpMethod.GET],
      integration: integration,
      authorizer: new HttpNoneAuthorizer(),
    });

    httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/calculator/echo`,
      methods: [HttpMethod.GET],
      integration: integration,
    });

    const echo2Route = httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/calculator/echo2/{x}`,
      methods: [HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/users`,
      methods: [HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/events`,
      methods: [HttpMethod.POST, HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/events/{eventId}`,
      methods: [HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: `${this.API_BASE_PATH}/events/{eventId}/join`,
      methods: [HttpMethod.POST],
      integration: integration,
    });

    new ARecord(this, "BeMyGuestApiAliasRecord", {
      zone: props.hostedZone,
      target: RecordTarget.fromAlias(
        new ApiGatewayv2DomainProperties(
          domain.regionalDomainName,
          domain.regionalHostedZoneId
        )
      ),
    });
  }
}
