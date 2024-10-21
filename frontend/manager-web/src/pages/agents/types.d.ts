import { IAgent } from "../../types";

export type ITeam = {
  teamId: number;
  teamName: string;
  agents: IAgent[];
};

export type ITeamShort = {
  teamId: number;
  name: string;
};

export interface IOption {
  label: string;
  value: number | string;
}

export interface ITag {
  id: number;
  status: string;
  name: string;
  value: number;
  lifetimeSeconds: number;
}
