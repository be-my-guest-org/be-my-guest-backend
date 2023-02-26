import { Construct } from "constructs";

import { AttributeType, BillingMode, Table } from "aws-cdk-lib/aws-dynamodb";

export interface DynamoDbTableProps {}

export class DynamoDbTable extends Construct {
  constructor(scope: Construct, id: string, props?: DynamoDbTableProps) {
    super(scope, id);

    const table = new Table(this, "be-my-guest-table", {
      partitionKey: { name: "pk", type: AttributeType.STRING },
      sortKey: { name: "sk", type: AttributeType.STRING },
      billingMode: BillingMode.PROVISIONED,
      readCapacity: 1,
      writeCapacity: 1,
    });

    table
      .autoScaleReadCapacity({ minCapacity: 1, maxCapacity: 10 })
      .scaleOnUtilization({ targetUtilizationPercent: 70 });
    table
      .autoScaleWriteCapacity({ minCapacity: 1, maxCapacity: 10 })
      .scaleOnUtilization({ targetUtilizationPercent: 70 });
  }
}
