import styled from "styled-components";
import { useFonts } from "hooks/useFonts";
import { getElFont, getTextWidth } from "utils/textMeasurer";
import React, { useEffect, useRef, useState } from "react";
import TooltipPopover from "components/tooltipPopover/TooltipPopover";

type Props = {
  children: string;
};

const FullNameTooltip = ({ children }: Props) => {
  const [hasTooltip, setHasTooltip] = useState(false);
  const containerRef = useRef(null);
  const isFontLoaded = useFonts("Inter light");

  useEffect(() => {
    if (isFontLoaded) {
      const font = getElFont(containerRef.current);
      const textWidth = getTextWidth(children, font);

      if (containerRef.current.offsetWidth + 1 < textWidth) {
        setHasTooltip(true);
      }
    }
  }, [isFontLoaded, children]);

  if (hasTooltip) {
    return (
      <StyledTooltip position="right" width={120} showOnHover tooltip={children}>
        <Container>{children}</Container>
      </StyledTooltip>
    );
  }

  return <Container ref={containerRef}>{children}</Container>;
};

export default FullNameTooltip;

const Container = styled.div`
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const StyledTooltip = styled(TooltipPopover)`
  text-align: center;
  padding: 5px 16px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 2px 8px ${({ theme }) => theme.colors.border.primary};
  ${({ theme }) => theme.typography.smallText1};
  border-radius: 4px;
`;
