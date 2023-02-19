import {
  CorsHttpMethod,
  HttpApi,
  HttpMethod,
} from "@aws-cdk/aws-apigatewayv2-alpha";
import { HttpLambdaIntegration } from "@aws-cdk/aws-apigatewayv2-integrations-alpha";
import { DotNetFunction } from "@xaaskit-cdk/aws-lambda-dotnet";
import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";

export interface ApiProps {}

export class Api extends Construct {
  constructor(scope: Construct, id: string, props?: ApiProps) {
    super(scope, id);

    const beMyGuestLambda = new DotNetFunction(this, "BeMyGuest", {
      projectDir: "../BeMyGuest/BeMyGuest.Api/src/BeMyGuest.Api",
    });

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
    });

    httpApi.addRoutes({
      path: "/calculator/echo",
      methods: [HttpMethod.GET],
      integration: integration,
    });

    httpApi.addRoutes({
      path: "/calculator/echo2/{x}",
      methods: [HttpMethod.GET],
      integration: integration,
    });

    new cdk.CfnOutput(this, "apiUrl", {
      value: httpApi.url!,
    });
  }
}
