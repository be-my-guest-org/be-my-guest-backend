import { DotNetFunction } from "@xaaskit-cdk/aws-lambda-dotnet";
import { Duration, RemovalPolicy } from "aws-cdk-lib";
import { LogGroup, RetentionDays } from "aws-cdk-lib/aws-logs";
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

    new LogGroup(this, `${id}LogGroup`, {
      logGroupName: `/aws/lambda/${lambdaFunction.functionName}`,
      removalPolicy: RemovalPolicy.DESTROY,
      retention: RetentionDays.THREE_MONTHS
    });

    this.lambdaFunction = lambdaFunction;
  }

  public addEnvironment(key: string, value: string) {
    this.lambdaFunction.addEnvironment(key, value);
  }
}
