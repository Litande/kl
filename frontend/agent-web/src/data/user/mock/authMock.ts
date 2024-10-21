import { Agent } from "data/user/types";
import { ROLES } from "router/enums";

export const authMock = async ({ email, password }) => {
  return new Promise<Agent>((resolve, reject) => {
    setTimeout(() => {
      const user: Agent = {
        email,
        authToken: "",
        id: "1",
        name: "Fred",
        role: ROLES.AGENT,
      };

      resolve(user);
    }, 1000);
  });
};
