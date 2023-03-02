import { DotNetFunction } from "@xaaskit-cdk/aws-lambda-dotnet";
import { Duration } from "aws-cdk-lib";
import { Construct } from "constructs";

export interface DotNetFunctionProps {
  readonly projectDir: string;
  readonly timeout?: Duration;
}

export class DotNetLambdaFunction extends Construct {
  public readonly lambdaFunction: DotNetFunction;

  constructor(scope: Construct, id: string, props: DotNetFunctionProps) {
    super(scope, id);

    const lambdaFunction = new DotNetFunction(this, id, {
      projectDir: props.projectDir,
      timeout: props.timeout ?? Duration.seconds(30),
    });

    this.lambdaFunction = lambdaFunction;

    lambdaFunction.addEnvironment;
  }

  public addEnvironment(key: string, value: string) {
    this.lambdaFunction.addEnvironment(key, value);
  }
}
