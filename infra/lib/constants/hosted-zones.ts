import { Environments } from "./constants";

export interface HostedZones {
  [key: string]: { name: string; id: string };
}

export const hostedZones: HostedZones = {
  [Environments.PROD]: {
    name: "jordangottardo.com",
    id: "TODO",
  },
  [Environments.DEV]: {
    name: "dev.be-my-guest.jordangottardo.com",
    id: "Z0302935DZ6A8MD4VY32",
  },
};
