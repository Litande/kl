import styled from "styled-components";

import TelephonySettings from "pages/settings/items/telephony/TelephonySettings";
import SettingItem from "pages/settings/SettingItem";
import FeedbackSetting from "pages/settings//items/feedback/FeedbackSetting";
import LeadAssignmentSetting from "pages/settings//items/leadAssignment/LeadAssignmentSetting";
import CallHoursSetting from "pages/settings//items/callHours/CallHoursSetting";
import ProductiveSetting from "pages/settings//items/productiveSettings/ProductiveSetting";
import DroppedCallSetting from "pages/settings//items/droppedCall/DroppedCallSetting";
import VoiceMailSetting from "pages/settings//items/voicemail/VoiceMailSetting";
import { PageTitle } from "components/layout/Layout";
import { SettingType } from "pages/settings/types";
import { SettingsProvider } from "pages/settings/ContextProvider";

const Settings = () => {
  return (
    <SettingsProvider>
      <Wrap>
        <PageTitle>Settings</PageTitle>
        <SettingsContainer>
          <SettingItem
            id={SettingType.Telephony}
            label={<SettingItemLabel>Telephony</SettingItemLabel>}
            Component={TelephonySettings}
          />
          <SettingItem
            id={SettingType.Feedback}
            Component={FeedbackSetting}
            label={<SettingItemLabel>Feedback</SettingItemLabel>}
          />
          <SettingItem
            id={SettingType.AgentPermanentLeadAssignment}
            Component={LeadAssignmentSetting}
            label={<SettingItemLabel>Agent permanent lead assignment</SettingItemLabel>}
          />
          <SettingItem
            id={SettingType.CallHours}
            Component={CallHoursSetting}
            label={<SettingItemLabel>Call Hours (lead&apos;s local time)</SettingItemLabel>}
          />
          <SettingItem
            id={SettingType.ProductiveDialer}
            Component={ProductiveSetting}
            label={<SettingItemLabel>Productivity Dialer Settings</SettingItemLabel>}
          />
          <SettingItem
            id={SettingType.DroppedCall}
            label={<SettingItemLabel>Dropped Call Settings</SettingItemLabel>}
            Component={DroppedCallSetting}
          />
          <SettingItem
            id={SettingType.VoiceMail}
            label={<SettingItemLabel>Voicemail Settings</SettingItemLabel>}
            Component={VoiceMailSetting}
          />
        </SettingsContainer>
      </Wrap>
    </SettingsProvider>
  );
};

export default Settings;

const Wrap = styled.div`
  display: flex;
  flex-direction: column;
  height: 90vh;
`;

const SettingsContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  padding: 0 0 4rem;
`;

const SettingItemLabel = styled.div`
  color: ${({ theme }) => theme.colors.fg.primary};
`;
