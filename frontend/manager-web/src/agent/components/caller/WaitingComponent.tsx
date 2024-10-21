import { useEffect, useState } from "react";
import styled from "styled-components";
import { typography } from "globalStyles/theme/fonts";
import { connectAction } from "agent/actions/connectionActions";

const WaitingComponent = () => {
  const [countSeconds, setCountSeconds] = useState(1);

  useEffect(() => {
    connectAction(1);
  }, []);

  useEffect(() => {
    const tempTimeout = setTimeout(() => {
      setCountSeconds(countSeconds + 1);
    }, 1000);
    return () => {
      clearTimeout(tempTimeout);
    };
  }, [countSeconds]);

  const getCounter = () => {
    const text = countSeconds === 1 ? "second" : "seconds";
    return countSeconds + " " + text;
  };

  return <Wrap>Waiting {getCounter()}</Wrap>;
};

export default WaitingComponent;

const Wrap = styled.div`
  text-align: left;
  ${typography.subtitle2}
`;
