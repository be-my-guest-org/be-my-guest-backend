import * as cdk from "aws-cdk-lib";
import {
  Certificate,
  CertificateValidation,
} from "aws-cdk-lib/aws-certificatemanager";
import { HostedZone } from "aws-cdk-lib/aws-route53";
import { Construct } from "constructs";
import { Constants } from "./constants/constants";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";
import { DotNetLambdaFunction } from "./constructs/dot-net-lambda-function";
import { DynamoDbTable } from "./constructs/dynamo-db-table";
import { GeoDataDynamoDbTable } from "./constructs/geo-data-dynamodb-table";

export class BeMyGuestStack extends cdk.Stack {
  private readonly TABLE_NAME_ENV_VAR = "DynamoDb__TableName";
  private readonly GEO_DATA_TABLE_NAME_ENV_VAR = "DynamoDb__GeoDataTableName";
  private readonly GSI1_NAME_ENV_VAR = "DynamoDb__Gsi1Name";
  private readonly HOSTED_ZONE_NAME = "jordangottardo.com";
  private readonly HOSTED_ZONE_ID = "Z027129813KQ3AFNBS4SO";

  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

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

    const geoDataDynamoDbTable = new GeoDataDynamoDbTable(
      this,
      "BeMyGuestGeoDataTable",
      {
        readWriteDataGrantees: [beMyGuestLambda.lambdaFunction],
      }
    );

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
      dynamoDbTable.gsiName
    );

    beMyGuestLambda.addEnvironment(
      this.GEO_DATA_TABLE_NAME_ENV_VAR,
      geoDataDynamoDbTable.tableName
    );

    const existingHostedZone = HostedZone.fromHostedZoneAttributes(
      this,
      "HostedZone",
      {
        hostedZoneId: this.HOSTED_ZONE_ID,
        zoneName: this.HOSTED_ZONE_NAME,
      }
    );

    const certificate = new Certificate(this, "BeMyGuestCertificate", {
      domainName: Constants.DOMAIN_NAME,
      validation: CertificateValidation.fromDns(existingHostedZone),
    });

    new Api(this, "BeMyGuestApi", {
      userPoolId: userPool.userPoolId,
      userPoolAppIntegrationClientIds: userPool.userPoolAppIntegrationClientIds,
      lambdaFunction: beMyGuestLambda.lambdaFunction,
      certificate: certificate,
      hostedZone: existingHostedZone,
      domainName: Constants.DOMAIN_NAME,
      subdomainName: Constants.SUBDOMAIN_NAME,
    });
  }
}
