import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import { Api } from "./constructs/Api";
import { CognitoUserPool } from "./constructs/CognitoUserPool";

export class BeMyGuestStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    new Api(this, "Api");
    new CognitoUserPool(this, "UserPool");
  }
}
