import * as cdk from "aws-cdk-lib";
import {
  Certificate,
  CertificateValidation,
} from "aws-cdk-lib/aws-certificatemanager";
import { HostedZone } from "aws-cdk-lib/aws-route53";
import { Construct } from "constructs";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";
import { DotNetLambdaFunction } from "./constructs/dot-net-lambda-function";
import { DynamoDbTable } from "./constructs/dynamo-db-table";

export class BeMyGuestStack extends cdk.Stack {
  private readonly TABLE_NAME_ENV_VAR = "DynamoDb__TableName";
  private readonly HOSTED_ZONE_ID = "Z027129813KQ3AFNBS4SO";
  private readonly DOMAIN_NAME = "be-my-guest.jordangottardo.com";

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
    });

    const beMyGuestLambda = new DotNetLambdaFunction(this, "BeMyGuestLambda", {
      projectDir: "../BeMyGuest/BeMyGuest.Api/src/BeMyGuest.Api",
    });

    new Api(this, "BeMyGuestApi", {
      userPoolId: userPool.userPoolId,
      userPoolAppIntegrationClientId: userPool.userPoolAppIntegrationClientId,
      lambdaFunction: beMyGuestLambda.lambdaFunction,
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

    const existingHostedZone = HostedZone.fromHostedZoneId(this, "HostedZone", this.HOSTED_ZONE_ID);

    new Certificate(this, "BeMyGuestCertificate", {
      domainName: this.DOMAIN_NAME,
      validation: CertificateValidation.fromDns(existingHostedZone)
    });
  }
}
