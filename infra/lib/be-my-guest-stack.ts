import { DotNetFunction } from "@xaaskit-cdk/aws-lambda-dotnet";
import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";
import { DynamoDbTable } from "./constructs/dynamo-db-table";

export class BeMyGuestStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const postUserConfirmationLambda = new DotNetFunction(this, "post-user-confirmation", {
      projectDir:
        "../PostConfirmationLambda/PostConfirmationLambda/src/PostConfirmationLambda",
    });

    const userPool = new CognitoUserPool(this, "UserPool", {
      postConfirmationLambda: postUserConfirmationLambda,
    });

    new Api(this, "Api", {
      userPoolId: userPool.userPoolId,
      userPoolAppIntegrationClientId: userPool.userPoolAppIntegrationClientId,
    });

    new DynamoDbTable(this, "Table");
  }
}
