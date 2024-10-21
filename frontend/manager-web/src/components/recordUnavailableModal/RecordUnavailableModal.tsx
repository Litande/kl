import { FC } from "react";
import styled from "styled-components";
import { typography } from "globalStyles/theme/fonts";

import ModalV2 from "components/modal/ModalV2";

type Props = {
  onClose: () => void;
};

const RecordUnavailableModal: FC<Props> = ({ onClose }) => {
  return (
    <ModalV2 title={`Record unavailable`} onCancel={onClose} hasCloseIcon>
      <Body>Please contact to admin/agent/manager</Body>
    </ModalV2>
  );
};

export default RecordUnavailableModal;

const Body = styled.div`
  box-sizing: border-box;
  display: flex;
  align-items: center;
  height: 100px;
  padding-top: 2rem;
  padding-bottom: 2rem;
  ${typography.body1}
`;
