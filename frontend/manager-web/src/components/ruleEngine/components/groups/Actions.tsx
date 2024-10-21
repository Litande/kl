import styled from "styled-components";

import { IRule } from "components/ruleEngine/types";
import { camelCaseToNormalStringFormat } from "components/ruleEngine/utils";

export default function Actions(props: Partial<IRule>) {
  if (!props?.ruleData) {
    return <Wrapper></Wrapper>;
  }
  const { actions } = props.ruleData;

  const renderFields = fields => {
    if (!fields.length) {
      return null;
    }
    const joinedFields = fields.map(field => field.value).join(", ");
    return <ColoredText>{joinedFields}</ColoredText>;
  };

  if (!actions) {
    return null;
  }

  return (
    <Wrapper>
      {actions.map((item, idx) => {
        return (
          <div key={item?.name + idx}>
            <b>{camelCaseToNormalStringFormat(item.name)} </b>
            {item?.actionOperation && (
              <span>{camelCaseToNormalStringFormat(item.actionOperation)} </span>
            )}
            {renderFields(item.fields || [])}
            {idx + 1 !== actions.length && <span>{" and"}</span>}
          </div>
        );
      })}
    </Wrapper>
  );
}

const Wrapper = styled.span`
  display: flex;
  flex-direction: column;
  flex: 3 1 25ch;
  justify-content: center;
  gap: 0 0.5ch;
  flex-wrap: wrap;
  ${({ theme }) => theme.typography.body1};

  b {
    ${({ theme }) => theme.typography.body2};
  }
`;

const ColoredText = styled.span`
  color: ${({ theme }) => theme.colors.leadGroups.darkBlue};
`;
