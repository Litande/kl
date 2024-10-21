import { FC } from "react";
import styled from "styled-components";

import ModalV2 from "components/modal/ModalV2";
import LoaderSpinner from "components/loader/LoaderSpinner";

type Props = {
  id: number;
};

const LoaderModal: FC<Props> = ({ id }) => {
  return (
    <ModalV2 title={`Record id ${id}`}>
      <Body>
        <LoaderSpinner />
      </Body>
    </ModalV2>
  );
};

export default LoaderModal;

const Body = styled.div`
  padding-top: 2rem;
  padding-bottom: 2rem;
`;
