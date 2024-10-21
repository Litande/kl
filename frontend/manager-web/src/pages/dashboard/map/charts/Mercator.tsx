import { MouseEventHandler, useCallback, useEffect, useMemo, useRef, useState } from "react";
import styled from "styled-components";
import { Mercator } from "@visx/geo";
import { Zoom, applyMatrixToPoint } from "@visx/zoom";
import { localPoint } from "@visx/event";
import { RectClipPath } from "@visx/clip-path";
import * as topojson from "topojson-client";
import classNames from "classnames";
import { ProvidedZoom, TransformMatrix } from "@visx/zoom/lib/types";

import topology from "../world-topo.json";
import CountryTooltip, { ICountryTooltip } from "pages/dashboard/map/tooltip/CountryTooltip";
import { IFeatureShape } from "pages/dashboard/map/types";
import { ICountryItem } from "pages/dashboard/callAnalysis/types";
import eventDomDispatcher from "services/events/EventDomDispatcher";
import {
  CHANGE_CALL_ANALYSIS_DATA,
  SELECT_CALL_ANALYSIS_ITEM,
  SET_SCALE_TO_GEO_MAP,
} from "pages/dashboard/actions";
import { GEO_CHART_HEIGHT, GEO_CHART_WIDTH } from "pages/dashboard/map/charts/consts";
import { createColorFunction } from "../utils";
import { defaultShadow } from "globalStyles/theme/border";

export const background = "#F3F8FF";
const COUNTRY_HOVER_COLOR = "#BCB6DB";
const MIN_AMOUNT_COUNTRY_COLOR = "#97b0fc";
const MAX_AMOUNT_COUNTRY_COLOR = "#134b94";
const UNKNOWN_COUNTRY_CODE = "ZZZ";

export type GeoMercatorProps = {
  width: number;
  height: number;
  events?: boolean;
};

const world = topojson.feature(topology, topology.objects.units) as {
  type: "FeatureCollection";
  features: IFeatureShape[];
};

const countryColor = {
  inactive: "#D8DCE6",
  active: "#BCB6DB",
};

interface ICallAnalysisData {
  [key: string]: ICountryItem;
}

const convertArrayToObjByCode = (data: ICountryItem[]): ICallAnalysisData =>
  data.reduce((acc, next) => {
    acc[next.code] = next;
    return acc;
  }, {}) as ICallAnalysisData;

const MercatorInternal = ({ width, height }: GeoMercatorProps) => {
  const zoomRef = useRef<ProvidedZoom<SVGSVGElement> & { transformMatrix: TransformMatrix }>();
  const { addEventListener, removeEventListener } = eventDomDispatcher();
  const [callAnalysisData, setCallAnalysisData] = useState<ICallAnalysisData>({});

  const getColor = useMemo(() => {
    const amountArray = Object.values(callAnalysisData).map(({ amount }) => amount);
    return createColorFunction(amountArray, MIN_AMOUNT_COUNTRY_COLOR, MAX_AMOUNT_COUNTRY_COLOR);
  }, [callAnalysisData]);

  const onCallAnalysisDataUpdate = (event: CustomEvent<{ data: ICountryItem[] }>) => {
    setCallAnalysisData(convertArrayToObjByCode(event.detail.data));
  };

  // center geo map to position of the element's DOMRect
  // in case the element is out of the visible view when the map is zoomed in
  const centerMap = (pos: { x: number; y: number }): boolean => {
    if (!zoomRef.current) {
      return false;
    }
    const { x, y } = pos;
    const shiftX = GEO_CHART_WIDTH / 2 - x;
    const isXShifted = Math.abs(shiftX) > GEO_CHART_WIDTH * 0.4;
    const shiftY = GEO_CHART_HEIGHT / 2 - y;
    const isYShifted = Math.abs(shiftY) > GEO_CHART_HEIGHT * 0.35;
    if (!isXShifted && !isYShifted) {
      return false;
    }
    // get scale ratio to compensate the shift
    const { scaleX, scaleY } = zoomRef.current.transformMatrix;
    zoomRef.current.translate({
      translateX: isXShifted ? shiftX / scaleX : 0,
      translateY: isYShifted ? shiftY / scaleY : 0,
    });
    return true;
  };

  const showTooltip = (code: string) => {
    let el = window.document.getElementById(code);
    if (!el) {
      console.warn(`Country with code [${code}] not found`);
      el = window.document.getElementById(UNKNOWN_COUNTRY_CODE);
      if (!el) {
        return;
      }
    }
    const domRects = el.getClientRects();
    if (typeof domRects !== "object" || !domRects[0]) {
      console.warn(`Country with code [${code}] :: getClientRects() issue`);
      return;
    }
    const domRect = domRects[0];

    const { x, y, width, height } = domRect;
    let parent = el.parentElement;
    // searching for parent svg
    while (!["svg", "body"].includes(parent.tagName)) {
      parent = parent.parentElement;
    }
    if (parent.tagName === "body") {
      // unexpected case
      alert("SVG not found");
      return;
    }
    // get parent svg position to do necessary position shift
    const svgDomRects = parent.getClientRects();
    if (typeof svgDomRects !== "object" || !svgDomRects[0]) {
      console.warn(`Searching svg tag for country code [${code}] :: getClientRects() issue`);
      return;
    }
    const svgDomRect = svgDomRects[0];

    const posX = x - svgDomRect.x + width / 2;
    const posY = y - svgDomRect.y + height / 4;

    if (
      centerMap({
        x: posX,
        y: posY,
      })
    ) {
      // when centering map is called, then the event loop should be over
      // before calling showTooltip again
      setTimeout(() => showTooltip(code), 400);
    } else {
      setSelectedCountry({
        data: callAnalysisData[code],
        feature: { id: code },
        x: posX,
        y: posY,
      });
    }
  };
  const onSelectCallAnalysisItem = (event: CustomEvent<{ code: string }>) => {
    const { code } = event.detail;
    setSelectedCountry(null);
    showTooltip(code);
  };

  const onSetScaleToGeoMap = (event: CustomEvent<{ scale: number }>) => {
    const { scale } = event.detail;
    if (zoomRef?.current) {
      zoomRef.current.scale({ scaleX: scale });
    }
  };

  useEffect(() => {
    addEventListener(CHANGE_CALL_ANALYSIS_DATA, onCallAnalysisDataUpdate);
    addEventListener(SELECT_CALL_ANALYSIS_ITEM, onSelectCallAnalysisItem);
    addEventListener(SET_SCALE_TO_GEO_MAP, onSetScaleToGeoMap);
    return () => {
      removeEventListener(CHANGE_CALL_ANALYSIS_DATA, onCallAnalysisDataUpdate);
      removeEventListener(SELECT_CALL_ANALYSIS_ITEM, onSelectCallAnalysisItem);
      removeEventListener(SET_SCALE_TO_GEO_MAP, onSetScaleToGeoMap);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [callAnalysisData]);

  const isActive = (code: string): boolean => !!callAnalysisData[code];

  const defineColor = (code: string): string => {
    if (isActive(code)) {
      const { amount } = callAnalysisData[code] || {};
      if (!amount) {
        return countryColor.active;
      } else {
        return (getColor && getColor(amount)) || countryColor.active;
      }
    } else {
      return countryColor.inactive;
    }
  };

  const [selectedCountry, setSelectedCountry] = useState<ICountryTooltip>();

  const resetSelectedCountry = useCallback(
    () => selectedCountry && setSelectedCountry(null),

    [selectedCountry, setSelectedCountry]
  );

  const hideTooltip =
    (handler: MouseEventHandler) =>
    (event): void => {
      resetSelectedCountry();
      handler(event);
    };
  const onClick = (event, data: IFeatureShape) => {
    setSelectedCountry({
      data: callAnalysisData[data.id],
      feature: data,
      x: event.nativeEvent.layerX,
      y: event.nativeEvent.layerY,
    });
  };

  const initialTransform = {
    scaleX: 0.7,
    scaleY: 0.7,
    translateX: 70,
    translateY: 150,
    skewX: 0,
    skewY: 0,
  };

  return width < 10 ? null : (
    <Zoom<SVGSVGElement>
      width={width}
      height={height}
      scaleXMin={1 / 2}
      scaleXMax={4}
      scaleYMin={1 / 2}
      scaleYMax={4}
      initialTransformMatrix={initialTransform}
      constrain={(transformMatrix, prevTransformMatrix) => {
        const scaleMaxSize = 3;
        const scaleMinSize = 0.7;
        const scaleCoefficient = Math.round(transformMatrix.scaleX) * 1.5;
        const min = applyMatrixToPoint(transformMatrix, { x: 0, y: 0 });
        const max = applyMatrixToPoint(transformMatrix, { x: width, y: height });
        if (transformMatrix.scaleX < scaleMinSize || transformMatrix.scaleX > scaleMaxSize) {
          return prevTransformMatrix;
        }
        if (max.x > width * scaleCoefficient || max.y > height * scaleCoefficient) {
          return prevTransformMatrix;
        }
        if (
          min.x < -(width * transformMatrix.scaleX) ||
          min.y < -(height * transformMatrix.scaleY)
        ) {
          return prevTransformMatrix;
        }
        return transformMatrix;
      }}
    >
      {zoom => {
        zoomRef.current = zoom;
        return (
          <Wrap
            width={width}
            onWheelCapture={resetSelectedCountry}
            onTouchStart={zoom.dragStart}
            onTouchMove={zoom.dragMove}
            onTouchEnd={zoom.dragEnd}
            onMouseDown={zoom.dragStart}
            onMouseMove={zoom.isDragging ? hideTooltip(zoom.dragMove) : zoom.dragMove}
            onMouseUp={zoom.dragEnd}
            onMouseLeave={() => {
              if (zoom.isDragging) zoom.dragEnd();
            }}
            onClick={resetSelectedCountry}
            onDoubleClick={event => {
              resetSelectedCountry();
              const point = localPoint(event) || { x: 0, y: 0 };
              zoom.scale({ scaleX: 1.5, scaleY: 1.5, point });
            }}
          >
            {selectedCountry && <CountryTooltip {...selectedCountry} />}
            <svg
              width={width}
              height={height}
              style={{ cursor: zoom.isDragging ? "grabbing" : "grab", touchAction: "none" }}
              ref={zoom.containerRef}
            >
              <RectClipPath id="zoom-clip" width={width} height={height} />
              <rect width={width} height={height} rx={14} fill={"transparent"} />
              <g
                transform={zoom.toString()}
                className={`scale-${zoom.transformMatrix.scaleX.toFixed(2)}`}
              >
                <Mercator<IFeatureShape> data={world.features}>
                  {mercator => (
                    <g>
                      {mercator.features.map(({ feature, path }, i) => (
                        <path
                          id={feature.id}
                          className={classNames({ active: isActive(feature.id) })}
                          key={`map-feature-${i}`}
                          d={path || ""}
                          fill={defineColor(feature.id)}
                          stroke={background}
                          strokeWidth={0.5}
                          onClick={event => {
                            if (isActive(feature.id)) {
                              event.stopPropagation();
                              onClick(event, feature);
                            }
                          }}
                        />
                      ))}
                    </g>
                  )}
                </Mercator>
              </g>
            </svg>
          </Wrap>
        );
      }}
    </Zoom>
  );
};

export const MercatorComponent = () => (
  <MercatorInternal width={GEO_CHART_WIDTH} height={GEO_CHART_HEIGHT} />
);

interface IWidth {
  width: number;
}

const Wrap = styled.div<IWidth>`
  position: relative;
  overflow: hidden;
  margin: 0.3rem;
  ${defaultShadow};
  min-height: 465px;
  min-width: ${props => {
    return props.width + "px";
  }};
  path.active:hover {
    fill: ${COUNTRY_HOVER_COLOR};
    cursor: pointer;
  }

  svg[style*="cursor: grab"] g {
    transition-property: transform;
    transition-duration: 0.2s;
  }
  svg[style*="cursor: grabbing"] g {
    transition-property: none;
    transition-duration: 0s;
  }
`;
