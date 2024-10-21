export enum ROUTES {
  DASHBOARD = "/",
  AUTH = "auth",
  TRACKING = "tracking",
  LEADS = "leads",
  LEADS_SEARCH = "leadsSearch",
  SETTINGS = "settings",
  RULES = "rules",
  AGENTS = "agents",
  CALL_RECORDINGS = "recordings",
  TAGS = "tags",
  NOT_FOUND = "*",
}

export enum RULES_NESTED_ROUTES {
  NEW_LEADS = "new_leads",
  MAIN = "main",
  LEAD_SCORING = "lead_scoring",
  AGENT_SCORING = "agent_scoring",
  STATUS = "status",
  API_RULES = "api_rules",
}

export enum ROLES {
  AGENT = "AGENT",
  MANAGER = "MANAGER",
  TEST = "TEST",
}
