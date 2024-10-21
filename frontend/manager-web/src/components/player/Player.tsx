import { useState, useEffect, useRef } from "react";
import moment from "moment";
import styled, { css } from "styled-components";

import ModalV2 from "components/modal/ModalV2";

import Progress from "./Progress";

type Props = {
  id: number;
  audioUrl: string;
  agentName: string;
  clientName: string;
  onClose: () => void;
  onNext?: () => void;
  onPrev?: () => void;
};

const Player = ({ id, clientName, audioUrl, agentName, onClose, onNext, onPrev }: Props) => {
  const [isPlaying, setIsPlaying] = useState(true);
  const [elapsed, setElapsed] = useState(0);
  const [duration, setDuration] = useState(0);
  const audioPlayer = useRef(null);

  useEffect(() => {
    let interval;

    if (isPlaying) {
      interval = () => {
        const _duration = Math.floor(audioPlayer?.current?.duration);
        const _elapsed = Math.floor(audioPlayer?.current?.currentTime);

        if (_duration === _elapsed) setIsPlaying(false);

        setDuration(_duration);
        setElapsed(_elapsed);
      };

      setInterval(interval, 100);
    }

    return () => {
      clearInterval(interval);
    };
  }, [isPlaying]);

  const togglePlay = () => {
    if (!isPlaying) {
      audioPlayer.current.play();
    } else {
      audioPlayer.current.pause();
    }
    setIsPlaying(prev => !prev);
  };

  const goBack = () => {
    setIsPlaying(true);
    onPrev();
  };

  const goForward = () => {
    setIsPlaying(true);
    onNext();
  };

  const handleRewind = ([value]: [number]) => {
    audioPlayer.current.currentTime = value;
  };

  return (
    <ModalV2 title={`Record id ${id}`} onCancel={onClose} hasCloseIcon>
      <audio src={audioUrl} ref={audioPlayer} autoPlay={true} />
      <PlayerHeader>
        <PlayerHeaderInfo>
          Client: <PlayerHeaderName>{clientName}</PlayerHeaderName>
        </PlayerHeaderInfo>
        <PlayerHeaderInfo>
          Agent: <PlayerHeaderName>{agentName}</PlayerHeaderName>
        </PlayerHeaderInfo>
      </PlayerHeader>
      <PlayerBody>
        <Progress min={0} current={elapsed} max={duration || 1} handleChange={handleRewind} />
      </PlayerBody>
      <Controls>
        <CurrentTime>{moment.utc(elapsed * 1000).format("HH:mm:ss")}</CurrentTime>
        <IconsWrap>
          {onPrev && <PrevIcon className="icon-back" onClick={goBack} />}
          {isPlaying ? (
            <PauseIcon className="icon-pause" onClick={togglePlay} />
          ) : (
            <PlayIcon className="icon-play" onClick={togglePlay} />
          )}
          {onNext && <NextIcon className="icon-back" onClick={goForward} />}
        </IconsWrap>
        <TotalTime>{moment.utc(duration * 1000).format("HH:mm:ss")}</TotalTime>
      </Controls>
    </ModalV2>
  );
};

export default Player;

const PlayerHeader = styled.div`
  display: flex;
  width: 100%;
  justify-content: space-between;
  ${({ theme }) => theme.typography.body1};
`;

const PlayerBody = styled.div`
  width: 100%;
  margin: 2rem 0 1rem;
`;

const PlayerHeaderInfo = styled.div``;

const PlayerHeaderName = styled.span`
  ${({ theme }) => theme.typography.body1};
  color: ${({ theme }) => theme.colors.btn.secondary};
`;

const Controls = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
`;

const IconsWrap = styled.div`
  display: flex;
  align-items: center;
`;

const iconCss = css`
  cursor: pointer;
  font-size: 1.5rem;
  color: ${({ theme }) => theme.colors.btn.player};
  &:hover {
    color: ${({ theme }) => theme.colors.btn.player_hovered};
  }
  &:active {
    color: ${({ theme }) => theme.colors.icons.secondary};
  }
`;

const PlayIcon = styled.i`
  ${iconCss}
  margin: 0 4rem;
  font-size: 1.8rem;
`;

const PauseIcon = styled.i`
  ${iconCss}
  margin: 0 4rem;
  font-size: 1.8rem;
`;

const PrevIcon = styled.i`
  ${iconCss}
`;
const NextIcon = styled.i`
  ${iconCss}
  transform: rotate(180deg);
`;

const CurrentTime = styled.div`
  width: 70px;
  ${({ theme }) => theme.typography.smallText2};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;
const TotalTime = styled.div`
  width: 70px;
  text-align: right;
  ${({ theme }) => theme.typography.smallText2};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;
