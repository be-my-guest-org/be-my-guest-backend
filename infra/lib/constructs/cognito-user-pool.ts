import { RemovalPolicy } from "aws-cdk-lib";
import {
  AccountRecovery,
  ProviderAttribute,
  UserPool,
  UserPoolClientIdentityProvider,
  UserPoolIdentityProviderGoogle,
  VerificationEmailStyle,
} from "aws-cdk-lib/aws-cognito";
import { Secret } from "aws-cdk-lib/aws-secretsmanager";
import { Construct } from "constructs";
import { Constants } from "../constants/constants";

export interface ApiProps {}

export class CognitoUserPool extends Construct {
  public readonly userPoolId: string;
  public readonly userPoolAppIntegrationClientId: string;

  constructor(scope: Construct, id: string, props?: ApiProps) {
    super(scope, id);

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
    });

    this.userPoolId = cognitoUserPool.userPoolId;

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
        clientSecretValue: Secret.fromSecretNameV2(
          this,
          "secret-id",
          Constants.SECRETS_NAME
        ).secretValueFromJson(Constants.GOOGLE_CLIENT_SECRET_NAME),
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

    this.userPoolAppIntegrationClientId = client.userPoolClientId;

    client.node.addDependency(userPoolIdentityProviderGoogle);
  }
}
