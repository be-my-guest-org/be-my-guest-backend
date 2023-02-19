import {
  CorsHttpMethod,
  HttpApi,
  HttpMethod,
  HttpNoneAuthorizer,
} from "@aws-cdk/aws-apigatewayv2-alpha";
import { HttpJwtAuthorizer } from "@aws-cdk/aws-apigatewayv2-authorizers-alpha";
import { HttpLambdaIntegration } from "@aws-cdk/aws-apigatewayv2-integrations-alpha";
import { DotNetFunction } from "@xaaskit-cdk/aws-lambda-dotnet";
import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";

export interface ApiProps {
  readonly userPoolId: string;
  readonly userPoolAppIntegrationClientId: string;
}

export class Api extends Construct {
  constructor(scope: Construct, id: string, props: ApiProps) {
    super(scope, id);

    const beMyGuestLambda = new DotNetFunction(this, "BeMyGuest", {
      projectDir: "../BeMyGuest/BeMyGuest.Api/src/BeMyGuest.Api",
    });

    const issuerUrl = `https://cognito-idp.${
      cdk.Stack.of(this).region
    }.amazonaws.com/${props.userPoolId}`;

    const cognitoJwtAuthorizer = new HttpJwtAuthorizer(
      "cognito-jwt-authorizer",
      issuerUrl,
      {
        jwtAudience: [props.userPoolAppIntegrationClientId],
      }
    );

    const httpApi = new HttpApi(this, "be-my-guest", {
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
    });

    const integration = new HttpLambdaIntegration(
      "integration",
      beMyGuestLambda
    );

    httpApi.addRoutes({
      path: "/",
      methods: [HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: "/calculator/add/{a}/{b}",
      methods: [HttpMethod.GET],
      integration: integration,
      authorizer: new HttpNoneAuthorizer()
    });

    httpApi.addRoutes({
      path: "/calculator/echo",
      methods: [HttpMethod.GET],
      integration: integration,
    });

    const echo2Route = httpApi.addRoutes({
      path: "/calculator/echo2/{x}",
      methods: [HttpMethod.GET],
      integration: integration,
    });

    new cdk.CfnOutput(this, "apiUrl", {
      value: httpApi.url!,
    });
  }
}
