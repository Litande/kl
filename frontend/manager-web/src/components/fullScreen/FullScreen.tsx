import { FC, useEffect } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { fullScreenActions, fullScreenStateSelector } from "./fullScreenSlice";
import { useDispatch, useSelector } from "react-redux";

export const FULL_SCREEN_CLASS = "full-screen";
const addFullScreen = () => {
  document.body.classList.add(FULL_SCREEN_CLASS);
};
const removeFullScreen = () => {
  document.body.classList.remove(FULL_SCREEN_CLASS);
};

export const FullScreen: FC = () => {
  const { isFullScreen } = useSelector(fullScreenStateSelector);
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const toggleFullScreen = () => {
    dispatch(fullScreenActions.toggleScreenState());
  };

  const keyPressHandler = e => {
    if (e.key === "Escape") {
      dispatch(fullScreenActions.toggleScreenState());
    }
  };

  useEffect(() => {
    isFullScreen ? addFullScreen() : removeFullScreen();
  }, [isFullScreen]);

  useEffect(() => {
    document.body.addEventListener("keyup", keyPressHandler);
    return () => {
      document.body.removeEventListener("keyup", keyPressHandler);
    };
  }, []);

  const clickHandler = () => {
    toggleFullScreen();
  };

  return (
    <Wrap onClick={clickHandler}>
      <ExpandIcon
        className={isFullScreen ? "icon-collapse" : "icon-expand"}
        title={t(`dashboard.useEscToCollapse`)}
      />
    </Wrap>
  );
};

const Wrap = styled.div`
  margin: 0 8px;
  padding: 0 8px;
  display: inline-block;
  cursor: pointer;

  i:hover {
    filter: brightness(50%);
  }
  i:active {
    filter: brightness(5%);
  }
`;

const ExpandIcon = styled.i`
  padding: 5px;
  font-size: 24px;
  color: ${({ theme }) => theme.colors.btn.secondary};
  border: 1px solid ${({ theme }) => theme.colors.btn.secondary};
  border-radius: 4px;
  cursor: pointer;
`;
