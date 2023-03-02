import { Construct } from "constructs";

import { AttributeType, BillingMode, Table } from "aws-cdk-lib/aws-dynamodb";
import { IGrantable } from "aws-cdk-lib/aws-iam";

export interface DynamoDbTableProps {
  readonly readWriteDataGrantees: IGrantable[];
}

export class DynamoDbTable extends Construct {
  public readonly tableName: string;

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

    props?.readWriteDataGrantees.forEach((grantee) => {
      table.grantReadWriteData(grantee);
    });

    this.tableName = table.tableName;
  }
}
