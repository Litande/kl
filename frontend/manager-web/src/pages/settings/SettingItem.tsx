import React, { useEffect, useState } from "react";
import styled from "styled-components";
import useToggle from "hooks/useToggle";
import { defaultBorder } from "globalStyles/theme/border";
import { typography } from "globalStyles/theme/fonts";
import CollapseItem from "components/collapseItem/CollapseItem";
import {
  ButtonSaveProps,
  FormControlProps,
  HEADER_HEIGHT,
  HeaderHelper,
} from "pages/settings/SettingsHelper";
import { SettingType } from "pages/settings/types";
import settingsApi from "services/api/settings";
import { useForm, useWatch } from "react-hook-form";
import isEqual from "lodash.isequal";

type ComponentProps<T> = {
  label: string | JSX.Element;
  id: SettingType;
  data?: T;
  Component: React.ComponentType<FormControlProps & ButtonSaveProps>;
  isOpen?: boolean;
};

const SettingItem = <T extends object>({ id, data, label, Component }: ComponentProps<T>) => {
  const { setValue, getValues, handleSubmit, control, trigger, reset } = useForm({
    mode: "onChange",
  });
  const [isCollapsed, setIsCollapsed] = useToggle(true);
  const [initialData, setInitialData] = useState(data);

  const watchFields = useWatch({ control });

  const onCollapse = () => {
    setIsCollapsed();
  };

  const submitForm = data => {
    if (id !== SettingType.CallHours) {
      setIsCollapsed();
    } else {
      setValue("callHours", data.callHours);
    }

    settingsApi.updateSetting(id, data).then(() => {
      setInitialData(data);
    });
  };

  const onCancel = () => {
    reset(initialData);
  };

  const handleItemRemove = () => {
    const formValues = getValues();
    const data = {
      country: formValues.country,
      callHours: formValues.callHours.filter(({ country, from, till }) => {
        return country && from && till;
      }),
    };

    settingsApi.updateSetting(id, data);
  };

  const onApply = (validate = true) => {
    if (validate) {
      handleSubmit(submitForm)();
    } else if (id === SettingType.CallHours) {
      handleItemRemove();
    }
  };

  useEffect(() => {
    settingsApi.getSetting(id).then(({ data }) => {
      reset(data);
      setInitialData(data);
    });
  }, [id, reset]);

  const getContent = () => {
    return (
      <Component
        control={control}
        trigger={trigger}
        areActionsAvailable={!isEqual(watchFields, initialData)}
        onApply={onApply}
        onCancel={onCancel}
      />
    );
  };

  return (
    <Wrap>
      <ContentWrap>
        <CollapseItem
          isOpen={!isCollapsed}
          header={<HeaderHelper label={label} isOpen={!isCollapsed} height={HEADER_HEIGHT} />}
          content={getContent()}
          onCollapse={onCollapse}
        />
      </ContentWrap>
    </Wrap>
  );
};

export default SettingItem;

const Wrap = styled.div`
  ${defaultBorder}
`;

const ContentWrap = styled.div`
  width: 100%;
  ${typography.body1}
`;
