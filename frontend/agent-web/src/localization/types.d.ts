import { namespaces } from "localization/constants";

declare module "react-i18next" {
  interface ICustomTypeOptions {
    defaultNS: typeof namespaces.translation;
  }
}
