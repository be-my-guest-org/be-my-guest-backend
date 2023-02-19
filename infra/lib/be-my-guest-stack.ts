import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";

export class BeMyGuestStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const userPool = new CognitoUserPool(this, "UserPool");
    new Api(this, "Api", {
      userPoolId: userPool.userPoolId,
      userPoolAppIntegrationClientId: userPool.userPoolAppIntegrationClientId,
    });
  }
}
