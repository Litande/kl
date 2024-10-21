import React from "react";
import styled from "styled-components";

import useToggle from "hooks/useToggle";

import { defaultBorder } from "globalStyles/theme/border";
import { typography } from "globalStyles/theme/fonts";

import CollapseItem from "components/collapseItem/CollapseItem";
import { HEADER_HEIGHT, HeaderHelper } from "./AgentsHelper";
import { IOption } from "./types";

export type BaseProps = {
  group: IOption;
};

type ComponentProps = BaseProps & {
  component: JSX.Element;
  isOpen?: boolean;
};

const GroupWrapper = (props: ComponentProps) => {
  const [isOpen, setIsOpen] = useToggle(props.isOpen);
  const onCollapse = () => {
    setIsOpen();
  };

  const getContent = () => {
    return props.component;
  };

  return (
    <Wrap>
      <ContentWrap>
        <CollapseItem
          isOpen={isOpen}
          header={<HeaderHelper group={props.group} isOpen={isOpen} height={HEADER_HEIGHT} />}
          content={getContent()}
          onCollapse={onCollapse}
        />
      </ContentWrap>
    </Wrap>
  );
};

export default GroupWrapper;

const Wrap = styled.div`
  ${defaultBorder}
`;

const ContentWrap = styled.div`
  width: 100%;
  ${typography.body1}
`;
