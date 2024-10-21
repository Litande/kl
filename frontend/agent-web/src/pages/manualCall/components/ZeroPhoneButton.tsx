import { FC } from "react";
import styled from "styled-components";

import useLongPress from "hooks/useLongPress";

import { PhoneButtonContainer } from "./PhoneButton";

type Props = {
  onClick: (symbol: string) => void;
};

const ZeroPhoneButton: FC<Props> = ({ onClick }) => {
  const onLongPress = () => onClick("+");
  const onPress = () => onClick("0");
  const longPressEvent = useLongPress(onLongPress, onPress);

  return (
    <PhoneButtonContainer {...longPressEvent}>
      <Wrap>
        <span>0</span>
        <span>+</span>
      </Wrap>
    </PhoneButtonContainer>
  );
};

export default ZeroPhoneButton;

const Wrap = styled.div`
  display: flex;
  flex-direction: column;
`;
