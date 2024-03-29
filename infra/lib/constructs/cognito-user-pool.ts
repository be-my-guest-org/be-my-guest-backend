import { Duration, RemovalPolicy } from "aws-cdk-lib";
import {
  AccountRecovery,
  ProviderAttribute,
  UserPool,
  UserPoolClientIdentityProvider,
  UserPoolIdentityProviderGoogle,
  VerificationEmailStyle,
} from "aws-cdk-lib/aws-cognito";
import { IFunction } from "aws-cdk-lib/aws-lambda";
import { Secret } from "aws-cdk-lib/aws-secretsmanager";
import { Construct } from "constructs";
import { Constants } from "../constants/constants";
import { cognitoDomainPrefixes } from "../constants/domain-names";

export interface CognitoUserPoolProps {
  readonly postConfirmationLambda: IFunction;
  readonly googleOauthClientId: string;
  readonly environmentName: string;
}

export class CognitoUserPool extends Construct {
  public readonly userPoolId: string;
  public readonly userPoolAppIntegrationClientIds: string[] = [];

  constructor(scope: Construct, id: string, props: CognitoUserPoolProps) {
    super(scope, id);

    const cognitoUserPool = new UserPool(this, id, {
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
        familyName: { required: false, mutable: true },
        givenName: { required: false, mutable: true },
        address: { required: false, mutable: true },
        birthdate: { required: false, mutable: true },
        gender: { required: false, mutable: true },
      },
      passwordPolicy: {
        minLength: 8,
        requireLowercase: true,
        requireUppercase: false,
        requireSymbols: true,
      },
      lambdaTriggers: {
        postConfirmation: props?.postConfirmationLambda,
      },
      accountRecovery: AccountRecovery.EMAIL_ONLY,
      removalPolicy: RemovalPolicy.DESTROY,
    });

    this.userPoolId = cognitoUserPool.userPoolId;

    cognitoUserPool.addDomain("cognito-domain", {
      cognitoDomain: {
        domainPrefix: cognitoDomainPrefixes[props.environmentName],
      },
    });

    const userPoolIdentityProviderGoogle = new UserPoolIdentityProviderGoogle(
      this,
      `${id}GoogleIdentityProvider`,
      {
        clientId: props.googleOauthClientId,
        clientSecretValue: Secret.fromSecretNameV2(
          this,
          "secret-id",
          Constants.SECRETS_NAME
        ).secretValueFromJson(Constants.GOOGLE_CLIENT_SECRET_NAME),
        userPool: cognitoUserPool,
        scopes: ["profile", "email", "openid"],
        attributeMapping: {
          email: ProviderAttribute.GOOGLE_EMAIL,
          familyName: ProviderAttribute.GOOGLE_FAMILY_NAME,
          givenName: ProviderAttribute.GOOGLE_GIVEN_NAME,
          gender: ProviderAttribute.GOOGLE_GENDER,
          birthdate: ProviderAttribute.GOOGLE_BIRTHDAYS,
          phoneNumber: ProviderAttribute.GOOGLE_PHONE_NUMBERS,
        },
      }
    );

    const client = cognitoUserPool.addClient(`${id}AppClient`, {
      authFlows: {
        userSrp: true,
        custom: true,
      },
      oAuth: {
        callbackUrls: ["exp://192.168.0.102:19000/--/"],
        logoutUrls: ["exp://192.168.0.102:19000/--/"],
        flows: {
          authorizationCodeGrant: true,
        },
      },
      supportedIdentityProviders: [UserPoolClientIdentityProvider.GOOGLE],
    });

    this.userPoolAppIntegrationClientIds.push(client.userPoolClientId);
    client.node.addDependency(userPoolIdentityProviderGoogle);

    const testClient = cognitoUserPool.addClient(`${id}TestAppClient`, {
      authFlows: {
        userSrp: true,
        custom: true,
      },
      oAuth: {
        callbackUrls: ["https://www.example.com"],
        logoutUrls: ["https://www.example.com"],
        flows: {
          implicitCodeGrant: true,
        },
      },
      supportedIdentityProviders: [UserPoolClientIdentityProvider.GOOGLE],
      accessTokenValidity: Duration.hours(8),
      idTokenValidity: Duration.hours(8)
    });

    this.userPoolAppIntegrationClientIds.push(testClient.userPoolClientId);
    testClient.node.addDependency(userPoolIdentityProviderGoogle);
  }
}
