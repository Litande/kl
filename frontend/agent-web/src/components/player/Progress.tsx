import styled from "styled-components";
import { Range } from "react-range";

type Props = {
  min: number;
  max: number;
  current: number;
  handleChange: (values: [number]) => void;
};

const Progress = ({ min, max, current, handleChange }: Props) => {
  return (
    <Range
      step={1}
      min={min}
      max={max}
      values={[current]}
      onChange={(values: [number]) => handleChange(values)}
      renderTrack={({ props, children }) => {
        const progress = Math.floor((current / max) * 100);
        const background = `linear-gradient(to right, #5294C3 ${progress}%, #d9d9d9 ${progress}% 100%)`;

        return (
          <Track
            {...props}
            style={{
              background,
            }}
          >
            {children}
          </Track>
        );
      }}
      renderThumb={({ props }) => <Caret {...props} />}
    />
  );
};

export default Progress;

const Track = styled.div`
  height: 0.375rem;
  width: 100%;
`;

const Caret = styled.div`
  position: relative;
  height: 1.5rem;
  width: 1.5rem;
  background-color: white;
  border-radius: 50%;
  box-shadow: 0 1px 2px ${({ theme }) => theme.colors.border.primary};

  &:after {
    content: "";
    position: absolute;
    top: 0.5rem;
    right: 0.5rem;
    width: 0.5rem;
    height: 0.5rem;
    border-radius: 50%;
    background: ${({ theme }) => theme.colors.btn.secondary};
  }
`;
