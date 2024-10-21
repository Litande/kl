import { useState, useEffect, useMemo } from "react";
import { AxiosResponse } from "axios";
import {
  CellClickedEvent,
  GetRowIdFunc,
  GetRowIdParams,
  GridApi,
  GridReadyEvent,
} from "ag-grid-community";
import styled from "styled-components";

import Grid from "components/grid/Grid";
import dashboardApi from "services/api/dashboard";
import useConnections from "services/websocket/useConnections";
import eventDomDispatcher from "services/events/EventDomDispatcher";
import { useMounted } from "hooks/useMounted";
import useCarousel from "hooks/useCarousel";
import { CHANGE_CALL_ANALYSIS_DATA, SELECT_CALL_ANALYSIS_ITEM } from "pages/dashboard/actions";
import columnDefs from "./columnsDefs";
import { ICountryItem, Period } from "./types";
import { STATISTIC_WS } from "services/websocket/const";
import { PeriodItem } from "pages/dashboard/styles";
import SwitchComponent from "components/switch/SwitchComponent";
import useToggle from "hooks/useToggle";
import { typography } from "globalStyles/theme/fonts";
import { defaultShadow } from "globalStyles/theme/border";
import { ANALYSIS_WIDTH } from "pages/dashboard/consts";

const periods = [Period.day, Period.week, Period.month];

const CAROUSEL_SHOW_NEXT_COUNTRY_DELAY = 5 * 1000; // 5 sec

const CallAnalysis = () => {
  const { dispatchEvent } = eventDomDispatcher();
  const [isLoading, setIsLoading] = useState(true);
  const [activePeriod, setActivePeriod] = useState(Period.week.value);
  const [data, setData] = useState<ICountryItem[]>([]);
  const [gridApi, setGridApi] = useState<GridApi<ICountryItem>>();
  const [isLive, toogleLiveMode] = useToggle(true);
  const { carouselIndex, setCarouselIndex, stopCarousel, toggleCarousel, debouncedStartCarousel } =
    useCarousel();
  const isMounted = useMounted();

  // sets data and dispatches event to share data with geo map or other components
  const updateData = (data: ICountryItem[]) => {
    setData(data);
    dispatchEvent(new CustomEvent(CHANGE_CALL_ANALYSIS_DATA, { detail: { data } }));

    // stop carousel and restart it after delay time
    stopCarousel();
    debouncedStartCarousel();
  };

  const selectPeriod = period => {
    setIsLoading(true);
    setActivePeriod(period);
    dashboardApi
      .getLeadsStats({ period })
      .then((response: AxiosResponse) => {
        response && response.data && updateData(response.data.items);
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const onGridReady = (params: GridReadyEvent) => {
    params.api.sizeColumnsToFit();
    setGridApi(params.api);
  };

  const onStatsUpdate = data => {
    const { items } = data.find(item => item.periodType === activePeriod);
    updateData(items);
  };

  useConnections(STATISTIC_WS, [{ chanelName: "call_analysis", onMessage: onStatsUpdate }], [data]);

  const getRowId = useMemo<GetRowIdFunc>(() => (params: GetRowIdParams) => params.data.code, []);

  const selectCountry = (e: CellClickedEvent<ICountryItem>) => {
    // send event to geo map to show tooltip
    dispatchEvent(new CustomEvent(SELECT_CALL_ANALYSIS_ITEM, { detail: { code: e.data.code } }));

    // temporarily stops carousel
    stopCarousel();
    debouncedStartCarousel();
  };

  const showNextRowInCarousel = () => {
    if (!gridApi) {
      return undefined;
    }
    const node = gridApi.getDisplayedRowAtIndex(carouselIndex);
    if (!node) {
      return setCarouselIndex(0);
    }
    node.setSelected(true);
    dispatchEvent(new CustomEvent(SELECT_CALL_ANALYSIS_ITEM, { detail: { code: node.data.code } }));
  };

  useEffect(() => {
    dashboardApi
      .getLeadsStats({ period: activePeriod })
      .then((response: AxiosResponse) => {
        response && response.data && updateData(response.data.items);
      })
      .finally(() => {
        setIsLoading(false);
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (carouselIndex > -1) {
      showNextRowInCarousel();
      setTimeout(() => {
        if (!isMounted) {
          return undefined;
        }
        setCarouselIndex(index => {
          if (index === -1 || !Array.isArray(data)) {
            return -1;
          } else {
            if (index + 1 >= data.length) {
              return 0;
            } else {
              return index + 1;
            }
          }
        });
      }, CAROUSEL_SHOW_NEXT_COUNTRY_DELAY);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [carouselIndex]);

  const updateLiveMode = () => {
    toogleLiveMode();
    toggleCarousel();
  };

  return (
    <Container>
      <Header>
        <Title>
          Call analysis <InfoIcon className="icon-info" />
          <LiveModeLabel>Live Mode</LiveModeLabel>
          <SwitchComponent isActive={isLive} onClick={updateLiveMode} />
        </Title>
        <PeriodsContainer>
          {periods.map(period => (
            <PeriodItem
              key={period.value}
              onClick={() => selectPeriod(period.value)}
              isActive={period.value === activePeriod}
            >
              {period.label}
            </PeriodItem>
          ))}
        </PeriodsContainer>
      </Header>
      <GridContainer
        isLoading={isLoading}
        rowData={data}
        columnDefs={columnDefs}
        onGridReady={onGridReady}
        getRowId={getRowId}
        rowSelection="single"
        onCellClicked={selectCountry}
      />
    </Container>
  );
};

export default CallAnalysis;

const Container = styled.div`
  flex: 1;
  margin: 0.3rem;
  ${defaultShadow};
  width: ${ANALYSIS_WIDTH}px;
  .ag-theme-alpine .ag-root-wrapper {
    border-radius: 0;
  }
`;

const Title = styled.h3`
  display: flex;
  align-items: center;
`;

const LiveModeLabel = styled.span`
  padding-right: 0.5rem;
  ${typography.smallText1}
`;

const InfoIcon = styled.i`
  margin: 0 22px 0 1rem;
  color: ${({ theme }) => theme.colors.fg.link};
`;

const Header = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-sizing: border-box;
  height: 60px;
  padding: 2rem 1rem;
`;

const PeriodsContainer = styled.div`
  display: flex;
  gap: 1rem;
`;

const GridContainer = styled(Grid)`
  //width: ${ANALYSIS_WIDTH}px;
  width: 100%;
  height: calc(100% - 64px);
  .ag-header {
    margin-left: 1px;
  }
  .ag-horizontal-left-spacer {
    visibility: hidden;
  }
  .ag-body-horizontal-scroll {
    visibility: hidden;
  }
  .ag-body-horizontal-scroll-viewport {
    overflow: hidden;
  }
  .ag-center-cols-viewport {
    overflow-x: hidden;
  }
`;
