import {
  CorsHttpMethod,
  HttpApi,
  HttpMethod,
} from "@aws-cdk/aws-apigatewayv2-alpha";
import { HttpLambdaIntegration } from "@aws-cdk/aws-apigatewayv2-integrations-alpha";
import { DotNetFunction } from "@xaaskit-cdk/aws-lambda-dotnet";
import * as cdk from "aws-cdk-lib";
import { RemovalPolicy, SecretValue } from "aws-cdk-lib";
import {
  AccountRecovery,
  ProviderAttribute,
  UserPool,
  UserPoolClientIdentityProvider,
  UserPoolIdentityProviderGoogle,
  VerificationEmailStyle,
} from "aws-cdk-lib/aws-cognito";
import { Construct } from "constructs";

export class BeMyGuestStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

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

    const secrets = new cdk.aws_secretsmanager.Secret(
      this,
      "be-my-guest-secrets",
      {
        description: "Be My Guest secrets",
        secretObjectValue: {
          googleClientSecret: SecretValue.unsafePlainText("dummy-google-client-secret"),
        },
      }
    );

    const cognitoUserPool = new UserPool(this, "be-my-guest-user-pool", {
      signInAliases: { email: true },
      selfSignUpEnabled: true,
      autoVerify: { email: true },

      userVerification: {
        emailSubject: "You need to verify your email",
        emailBody:
          "Thanks for signing up! Click on the URL to confirm your account: {##Verify Email##}", //This placeholder is a must if email is selected as preferred verification method
        emailStyle: VerificationEmailStyle.LINK,
      },
      standardAttributes: {
        familyName: { required: false, mutable: false },
        givenName: { required: false, mutable: false },
        address: { required: false, mutable: true },
      },
      passwordPolicy: {
        minLength: 8,
        requireLowercase: true,
        requireUppercase: false,
        requireSymbols: true,
      },
      accountRecovery: AccountRecovery.EMAIL_ONLY,
      removalPolicy: RemovalPolicy.DESTROY,

      // custom_attributes : {
      //     "tenant_id": cognito.StringAttribute(min_len=10, max_len=15, mutable=False),
      //     "created_at": cognito.DateTimeAttribute(),
      //     "employee_id": cognito.NumberAttribute(min=1, max=100, mutable=False),
      //     "is_admin": cognito.BooleanAttribute(mutable=True),
      // account_recovery=cognito.AccountRecovery.EMAIL_ONLY,
      // removal_policy=core.RemovalPolicy.DESTROY
    });

    cognitoUserPool.addDomain("cognito-domain", {
      cognitoDomain: {
        domainPrefix: "be-my-guest",
      },
    });

    const userPoolIdentityProviderGoogle = new UserPoolIdentityProviderGoogle(
      this,
      "be-my-guest-user-pool-idp-google",
      {
        clientId:
          "696503683561-n84jq1davll1n4op87frvlj70c6f54dt.apps.googleusercontent.com",
        // clientSecret: "undefined",
        clientSecretValue: secrets.secretValueFromJson("googleClientSecret"),
        // clientSecretValue: secrets.secretValue,
        userPool: cognitoUserPool,
        scopes: ["profile", "email", "openid"],
        attributeMapping: {
          email: ProviderAttribute.GOOGLE_EMAIL,
        },
      }
    );

    const client = cognitoUserPool.addClient("app-client", {
      authFlows: {
        userSrp: true,
        custom: true,
      },
      oAuth: {
        callbackUrls: ["https://www.example.com/cb"],
        logoutUrls: ["https://www.example.com/signout"],
        flows: {
          implicitCodeGrant: true,
        },
      },
      supportedIdentityProviders: [UserPoolClientIdentityProvider.GOOGLE],
    });

    client.node.addDependency(userPoolIdentityProviderGoogle);
  }
}
