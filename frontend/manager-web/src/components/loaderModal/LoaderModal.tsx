import { FC } from "react";
import styled from "styled-components";

import ModalV2 from "components/modal/ModalV2";
import Loader from "../loader/Loader";

type Props = {
  id: number;
};

const LoaderModal: FC<Props> = ({ id }) => {
  return (
    <ModalV2 title={`Record id ${id}`}>
      <Body>
        <Loader />
      </Body>
    </ModalV2>
  );
};

export default LoaderModal;

const Body = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100px;
`;
