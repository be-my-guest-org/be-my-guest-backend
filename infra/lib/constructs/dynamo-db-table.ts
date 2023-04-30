import { Construct } from "constructs";

import { RemovalPolicy } from "aws-cdk-lib";
import { AttributeType, BillingMode, Table } from "aws-cdk-lib/aws-dynamodb";
import { IGrantable } from "aws-cdk-lib/aws-iam";

export interface DynamoDbTableProps {
  readonly readWriteDataGrantees: IGrantable[];
}

export class DynamoDbTable extends Construct {
  public readonly tableName: string;
  public readonly gsi1Name: string;
  public readonly gsi2Name: string;

  constructor(scope: Construct, id: string, props?: DynamoDbTableProps) {
    super(scope, id);

    const table = new Table(this, id, {
      partitionKey: { name: "pk", type: AttributeType.NUMBER },
      sortKey: { name: "sk", type: AttributeType.STRING },
      billingMode: BillingMode.PROVISIONED,
      readCapacity: 1,
      writeCapacity: 1,
      removalPolicy: RemovalPolicy.DESTROY,
    });

    this.gsi1Name = "gsi1";
    this.gsi2Name = "gsi2";

    table.addLocalSecondaryIndex({
      indexName: "geohash-index",
      sortKey: { name: "geohash", type: AttributeType.NUMBER },
    });

    table.addGlobalSecondaryIndex({
      indexName: this.gsi1Name,
      partitionKey: { name: "gsi1pk", type: AttributeType.STRING },
      sortKey: {
        name: "sk",
        type: AttributeType.STRING,
      },
      readCapacity: 1,
      writeCapacity: 1,
    });

    table.addGlobalSecondaryIndex({
      indexName: this.gsi2Name,
      partitionKey: { name: "gsi2pk", type: AttributeType.STRING },
      sortKey: {
        name: "sk",
        type: AttributeType.STRING,
      },
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
