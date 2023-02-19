import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import { Api } from "./constructs/api";
import { CognitoUserPool } from "./constructs/cognito-user-pool";

export class BeMyGuestStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    new Api(this, "Api");
    new CognitoUserPool(this, "UserPool");
  }
}
