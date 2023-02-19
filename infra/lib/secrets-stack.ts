import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import { Constants } from "./constants/constants";
export const cfnExports = { environmentSecretsArn: "environmentSecretsArn" };

export class SecretsStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const secrets = new cdk.aws_secretsmanager.Secret(
      this,
      "be-my-guest-secrets",
      {
        description: "Be My Guest secrets",
        secretName: Constants.SECRETS_NAME,
      }
    );
  }
}
