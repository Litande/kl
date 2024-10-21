import { FC, useEffect, useState } from "react";
import styled from "styled-components";

import ModalV2, { ModalContent } from "components/modal/ModalV2";
import Button from "components/button/Button";
import { useAgent } from "data/user/useAgent";

type Props = {
  onClose: () => void;
  onBack: () => void;
  onSubmit?: (comment: string) => void;
};

const NewCommentModal: FC<Props> = ({ onClose, onBack, onSubmit }) => {
  const { agent } = useAgent();
  const [comment, setComment] = useState("");

  const backHandler = () => {
    onBack();
  };

  const submitHandler = () => {
    agent.lead?.setComment(comment);
    onSubmit && onSubmit(comment);
  };

  const handleCommentChange = event => {
    setComment(event.target.value);
  };

  useEffect(() => {
    const subscribe = agent.lead?.comment.subscribe(value => {
      setComment(value);
    });
    return () => {
      subscribe?.unsubscribe();
    };
  }, [agent.lead]);

  const isSubmitButtonEnable = comment.length > 0;

  return (
    <StyledModal title={`New Comments`} onCancel={onClose} hasCloseIcon>
      <Body>
        <StyledTextarea onChange={handleCommentChange} />
      </Body>
      <Footer>
        <ButtonsContainer>
          <Button onClick={backHandler}>Back</Button>
          <Button disabled={!isSubmitButtonEnable} onClick={submitHandler}>
            Submit
          </Button>
        </ButtonsContainer>
      </Footer>
    </StyledModal>
  );
};

export default NewCommentModal;

const ButtonsContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-left: 16px;
  padding-right: 16px;
  width: 100%;
  gap: 16px;
`;

const Body = styled.div`
  padding: 16px;
`;

const Footer = styled.div`
  display: flex;
  height: 68px;
  width: 100%;
  border-top: 1px solid rgba(0, 0, 0, 0.12);
`;

const StyledModal = styled(ModalV2)`
  min-width: 580px;
  width: fit-content;

  ${ModalContent} {
    padding: 0;
  }
`;

const StyledTextarea = styled.textarea`
  width: 918px;
  height: 137px;
  border: 1px solid rgba(0, 0, 0, 0.12);
  padding: 10px 16px;
  border-radius: 4px;
  ${({ theme }) => theme.typography.body1}
`;
