import { Environments } from "./constants";

export interface DomainNames {
  [key: string]: string;
}

export const domainNames: DomainNames = {
  [Environments.PROD]: "be-my-guest.jordangottardo.com",
  [Environments.DEV]: "dev.be-my-guest.jordangottardo.com",
};

export const cognitoDomainPrefixes: DomainNames = {
  [Environments.PROD]: "be-my-guest",
  [Environments.DEV]: "dev-be-my-guest",
};