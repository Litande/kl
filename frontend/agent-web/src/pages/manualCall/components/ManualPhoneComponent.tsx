import React, { ChangeEvent, useMemo, useState } from "react";
import styled from "styled-components";

import { phoneSymbols } from "data/user/types";
import useAgentStatus from "hooks/useAgentStatus";
import Button from "components/button/Button";
import ErrorNotification from "components/error/ErrorNotification";
import { handleMicrophonePermission, MICROPHONE_ERROR } from "utils/microphoneUtils";
import { NO_INTERNET_CONNECTION_ERROR, isConnectionOnline } from "components/connection/utils";

import PhoneButton from "./PhoneButton";
import ZeroPhoneButton from "./ZeroPhoneButton";
import { useAgent } from "data/user/useAgent";

const ManualPhoneComponent = () => {
  const { isManualCallAvailable } = useAgentStatus();
  const { agent } = useAgent();
  const [phoneNumber, setPhoneNumber] = useState("");

  const handlePhoneButtonClick = symbol =>
    setPhoneNumber(currentPhoneNumber => currentPhoneNumber + symbol);
  const handleClearButtonClick = () => setPhoneNumber("");
  const handleDialButtonClick = async () => {
    if (isConnectionOnline()) {
      await handleMicrophonePermission(
        () => {
          setError(null);
          agent.startManualCall(phoneNumber);
        },
        () => setError(MICROPHONE_ERROR)
      );
    } else {
      setError(NO_INTERNET_CONNECTION_ERROR);
    }
  };
  const [error, setError] = useState(null);

  const onChange = (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    const isValid = value.split("").every(char => phoneSymbols.includes(char));

    if (isValid) {
      setPhoneNumber(value);
    }
  };

  const renderPhoneSymbols = useMemo(
    () =>
      phoneSymbols
        .filter(number => number !== "+")
        .map(symbol =>
          symbol === "0" ? (
            <ZeroPhoneButton
              data-testid="phone-button-0"
              key={symbol}
              onClick={handlePhoneButtonClick}
            />
          ) : (
            <PhoneButton
              data-testid={`phone-button-${symbol}`}
              key={symbol}
              value={symbol}
              onClick={() => handlePhoneButtonClick(symbol)}
            />
          )
        ),
    []
  );

  return (
    <Container>
      <StyledInput
        placeholder={"+123456789"}
        value={phoneNumber}
        data-testid={`phone-number`}
        onChange={onChange}
      />
      <Divider />
      <MiddleContainer>{renderPhoneSymbols}</MiddleContainer>
      <Divider />
      <BottomContainer>
        <Button
          disabled={!isManualCallAvailable || phoneNumber.length === 0}
          onClick={handleDialButtonClick}
          data-testid="kl-button"
        >
          Dial
        </Button>
        <Button variant="secondary" onClick={handleClearButtonClick}>
          Clear
        </Button>
      </BottomContainer>
      {error && <ErrorNotification error={error} />}
    </Container>
  );
};

export default ManualPhoneComponent;

const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1rem 62px 1rem;
  width: 341px;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 6px;
  gap: 0.5rem;
`;

const StyledInput = styled.input`
  min-height: 40px;
  font-family: "Inter medium";
  font-size: 32px;
  line-height: 39px;
  font-weight: 700;
  border: none;
  width: 100%;
  text-align: center;
  :focus {
    outline: none;
  }
`;

const MiddleContainer = styled.div`
  display: grid;
  grid-template-columns: auto auto auto;
  gap: 15px;
  margin: 1rem 0 1rem 0;
`;

const BottomContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 17px;

  button {
    width: 217px;
  }
`;

const Divider = styled.div`
  height: 1px;
  width: 216px;
  background: rgba(0, 0, 0, 0.12);
`;
