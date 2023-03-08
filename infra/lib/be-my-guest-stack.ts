import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";
import { DotNetLambdaFunction } from "./constructs/dot-net-lambda-function";
import { DynamoDbTable } from "./constructs/dynamo-db-table";

export class BeMyGuestStack extends cdk.Stack {
  private readonly TABLE_NAME_ENV_VAR = "TABLE_NAME";

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

    const beMyGuestLambda = new DotNetLambdaFunction(
      this,
      "BeMyGuestLambda",
      {
        projectDir: "../BeMyGuest/BeMyGuest.Api/src/BeMyGuest.Api",
      }
    );

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
  }
}
