import styled from "styled-components";

import { IRule } from "components/ruleEngine/types";
import { getCountryNameById, camelCaseToNormalStringFormat } from "components/ruleEngine/utils";

export default function Conditions(props: Partial<IRule>) {
  if (!props?.ruleData) {
    return <Wrapper></Wrapper>;
  }

  const { groups, operator } = props.ruleData.combination;

  const renderFields = (fields, name: string) => {
    if (!fields.length) {
      return null;
    }
    const joinedFields = fields
      .map(field => {
        if (name === "Country") {
          return field.value
            .split(",")
            .map(id => `"${getCountryNameById(id)}"`)
            .join(", ");
        }

        return field.value;
      })
      .join(" ");
    return <ColoredText>{joinedFields}</ColoredText>;
  };

  if (!groups) {
    return null;
  }

  return (
    <Wrapper>
      {groups.map((item, idx) => {
        return (
          <div key={item?.name + idx}>
            <b>{camelCaseToNormalStringFormat(item.name)} </b>
            {item?.comparisonOperation && (
              <span>{camelCaseToNormalStringFormat(item.comparisonOperation)} </span>
            )}
            {renderFields(item.fields || [], item.name)}
            {idx + 1 !== groups.length && <span> {operator}</span>}
          </div>
        );
      })}
    </Wrapper>
  );
}

const Wrapper = styled.span`
  display: flex;
  flex-direction: column;
  flex-wrap: wrap;
  gap: 0 0.5ch;
  flex: 3 1 30ch;
  ${({ theme }) => theme.typography.body1};

  b {
    ${({ theme }) => theme.typography.body2};
  }
`;

const ColoredText = styled.span`
  color: ${({ theme }) => theme.colors.leadGroups.darkBlue};
`;
