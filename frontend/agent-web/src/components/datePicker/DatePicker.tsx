import { useCallback, useState, useEffect, useRef } from "react";
import styled from "styled-components";
import Calendar from "react-calendar";
import moment from "moment";
import { useOutsideAlerter } from "hooks/useOutsideAlterer";

type Props = {
  initialDate?: Date | string;
  className?: string;
  label?: string;
  onSelect?: (date: Date) => void;
  showTime?: boolean;
};

const DatePicker = ({ initialDate, className, label, onSelect, showTime = false }: Props) => {
  const [isShown, setIsShown] = useState(false);
  const [date, setDate] = useState<Date>(
    moment(initialDate || new Date())
      .set("minute", 0)
      .toDate()
  );
  const containerRef = useRef(null);

  useOutsideAlerter(
    containerRef,
    () => {
      hideCalendar();
    },
    [isShown, date]
  );

  useEffect(() => {
    setDate(moment(initialDate).toDate());
  }, [initialDate]);

  const toggleCalendar = e => {
    e.stopPropagation();
    setIsShown(!isShown);
  };

  const hideCalendar = useCallback(() => {
    setIsShown(false);
  }, [isShown, date]);

  const handleDateChange = (stepType, step) => {
    const d = moment(date).add(step, stepType).toDate();

    setDate(d);
    onSelect(d);
  };

  const renderYear = () => (
    <HorizontalPicker>
      <ArrowLeft className="icon-ic-arrowDown" onClick={() => handleDateChange("years", -1)} />
      {moment(date).format("yyyy")}
      <ArrowRight className="icon-ic-arrowDown" onClick={() => handleDateChange("years", 1)} />
    </HorizontalPicker>
  );

  const renderMonth = () => (
    <HorizontalPicker>
      <ArrowLeft className="icon-ic-arrowDown" onClick={() => handleDateChange("months", -1)} />
      {moment(date).format("MMMM")}
      <ArrowRight className="icon-ic-arrowDown" onClick={() => handleDateChange("months", 1)} />
    </HorizontalPicker>
  );

  const renderTime = () => (
    <HorizontalPicker>
      <ArrowLeft className="icon-ic-arrowDown" onClick={() => handleDateChange("minutes", -30)} />
      {moment(date).format("HH:mm")}
      <ArrowRight className="icon-ic-arrowDown" onClick={() => handleDateChange("minutes", 30)} />
    </HorizontalPicker>
  );

  const renderDate = () => {
    if (!initialDate) return <Placeholder>Select...</Placeholder>;

    return showTime ? moment(date).format("HH:mm DD/MM/YYYY") : moment(date).format("DD/MM/YYYY");
  };

  const renderField = () => {
    return (
      <FieldContainer>
        {label && <Label>{label}</Label>}
        <Field isActive={isShown} onClick={toggleCalendar}>
          {renderDate()}
          <CalendarIcon className="icon-ic-calendar" isActive={isShown} />
        </Field>
      </FieldContainer>
    );
  };

  const renderCalendar = () => (
    <CalendarContainer>
      {showTime && renderTime()}
      {renderYear()}
      {renderMonth()}
      <StyledCalendar
        value={date}
        onChange={(newDate: Date) => {
          const [hours, minutes] = moment(date).format("HH:mm").split(":");
          const selectedDate = moment(newDate)
            .set("minute", Number(minutes))
            .set("hour", Number(hours))
            .toDate();

          setDate(selectedDate);
          onSelect(selectedDate);
        }}
        showNavigation={false}
        formatShortWeekday={(locale, date) => moment(date).format("ddd").substring(0, 1)}
      />
    </CalendarContainer>
  );

  return (
    <Container ref={containerRef} className={className}>
      {renderField()}
      {isShown && renderCalendar()}
    </Container>
  );
};

export default DatePicker;

const Container = styled.div`
  position: relative;
  ${({ theme }) => theme.typography.body1}
`;

const FieldContainer = styled.div`
  display: flex;
  flex-direction: column;
`;

const Label = styled.div`
  padding: 0 0 3px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_disabled};
  text-transform: uppercase;
`;

const Field = styled.div<{ isActive: boolean }>`
  position: relative;
  box-sizing: border-box;
  height: 36px;
  padding: 8px 35px 8px 8px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border: ${({ isActive, theme }) =>
    isActive ? `1px solid ${theme.colors.fg.link}` : `1px solid ${theme.colors.border.primary}`};
  border-radius: 4px;
  ${({ theme }) => theme.typography.body1}
`;
const CalendarIcon = styled.i<{ isActive }>`
  position: absolute;
  top: 6px;
  right: 4px;
  font-size: 24px;
  color: ${({ theme, isActive }) =>
    isActive ? theme.colors.fg.link : theme.colors.fg.secondary_light};
`;

const StyledCalendar = styled(Calendar)`
  border: none;
  ${({ theme }) => theme.typography.body1}

  button {
    border-radius: initial;
    height: auto;
    min-width: auto;
    font-size: initial;
    ${({ theme }) => theme.typography.body1}
    color: ${({ theme }) => theme.colors.fg.secondary};
  }

  .react-calendar__month-view__days__day--weekend {
    color: ${({ theme }) => theme.colors.fg.secondary};
    background: transparent;
  }

  .react-calendar__month-view__weekdays__weekday {
    & abbr {
      text-decoration: none;
    }
  }

  .react-calendar__tile--active {
    background: ${({ theme }) => theme.colors.bg.pageHeader};
  }
`;

const HorizontalPicker = styled.div`
  position: relative;
  margin: 0 0 30px;
  text-align: center;
`;

const ArrowRight = styled.i`
  position: absolute;
  transform: rotate(270deg);
  right: 0;
  font-size: 20px;
  line-height: 17px;
  color: ${({ theme }) => theme.colors.btn.secondary};
  cursor: pointer;
`;
const ArrowLeft = styled.i`
  position: absolute;
  transform: rotate(90deg);
  left: 0;
  font-size: 20px;
  line-height: 17px;
  color: ${({ theme }) => theme.colors.btn.secondary};
  cursor: pointer;
`;

const CalendarContainer = styled.div`
  z-index: 100;
  position: absolute;
  right: 0;
  left: 0;
  min-width: 200px;
  padding: 16px;

  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 4px 4px rgba(0, 0, 0, 0.25);
  border-radius: 4px;
`;

const Placeholder = styled.span`
  opacity: 0.3;
`;
