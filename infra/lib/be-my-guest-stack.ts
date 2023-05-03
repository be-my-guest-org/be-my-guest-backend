import * as cdk from "aws-cdk-lib";
import {
  Certificate,
  CertificateValidation,
} from "aws-cdk-lib/aws-certificatemanager";
import { HostedZone } from "aws-cdk-lib/aws-route53";
import { Construct } from "constructs";
import { Constants, Environments } from "./constants/constants";
import { domainNames } from "./constants/domain-names";
import { hostedZones } from "./constants/hosted-zones";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";
import { DotNetLambdaFunction } from "./constructs/dot-net-lambda-function";
import { DynamoDbTable } from "./constructs/dynamo-db-table";

export class BeMyGuestStack extends cdk.Stack {
  private readonly TABLE_NAME_ENV_VAR = "DynamoDb__TableName";
  private readonly GSI1_NAME_ENV_VAR = "DynamoDb__Gsi1Name";
  private readonly GSI2_NAME_ENV_VAR = "DynamoDb__Gsi2Name";

  constructor(scope: Construct, id: string, props: cdk.StackProps) {
    super(scope, id, props);

    const awsAccount = props.env?.account ?? Environments.DEV;

    const postUserConfirmationLambda = new DotNetLambdaFunction(
      this,
      "PostUserConfirmationLambda",
      {
        projectDir:
          "../PostConfirmationLambda/PostConfirmationLambda/src/PostConfirmationLambda",
      }
    );

    const userPool = new CognitoUserPool(this, "BeMyGuestUserPool", {
      postConfirmationLambda: postUserConfirmationLambda.lambdaFunction,
      googleOauthClientId: Constants.GOOGLE_OAUTH_CLIENT_ID,
      environmentName: awsAccount,
    });

    const beMyGuestLambda = new DotNetLambdaFunction(this, "BeMyGuestLambda", {
      projectDir: "../BeMyGuest/BeMyGuest.Api/src/BeMyGuest.Api",
    });

    const dynamoDbTable = new DynamoDbTable(this, "BeMyGuestTable", {
      readWriteDataGrantees: [
        postUserConfirmationLambda.lambdaFunction,
        beMyGuestLambda.lambdaFunction,
      ],
    });

    postUserConfirmationLambda.addEnvironment(
      this.TABLE_NAME_ENV_VAR,
      dynamoDbTable.tableName
    );

    beMyGuestLambda.addEnvironment(
      this.TABLE_NAME_ENV_VAR,
      dynamoDbTable.tableName
    );

    beMyGuestLambda.addEnvironment(
      this.GSI1_NAME_ENV_VAR,
      dynamoDbTable.gsi1Name
    );

    beMyGuestLambda.addEnvironment(
      this.GSI2_NAME_ENV_VAR,
      dynamoDbTable.gsi2Name
    );

    const existingHostedZone = HostedZone.fromHostedZoneAttributes(
      this,
      "HostedZone",
      {
        hostedZoneId: hostedZones[awsAccount].id,
        zoneName: hostedZones[awsAccount].name,
      }
    );

    const certificate = new Certificate(this, "BeMyGuestCertificate", {
      domainName: domainNames[awsAccount],
      validation: CertificateValidation.fromDns(existingHostedZone),
    });

    new Api(this, "BeMyGuestApi", {
      userPoolId: userPool.userPoolId,
      userPoolAppIntegrationClientIds: userPool.userPoolAppIntegrationClientIds,
      lambdaFunction: beMyGuestLambda.lambdaFunction,
      certificate: certificate,
      hostedZone: existingHostedZone,
      domainName: domainNames[awsAccount],
    });
  }
}
