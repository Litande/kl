type CallbackType = (event?: CustomEvent) => void;

type EventType = {
  eventName: string;
  callback: CallbackType;
  isDomEvent?: boolean;
};

let eventList: Array<EventType>;
const eventDomDispatcher = () => {
  if (!eventList) {
    eventList = new Array<EventType>();
  }

  const findEvent = (event: EventType): EventType => {
    return eventList.find(e => e.eventName === event.eventName && e.callback === event.callback);
  };

  const addEventListener = (eventName: string, callback: CallbackType, isDomEvent = true) => {
    if (findEvent({ eventName, callback }) === undefined) {
      isDomEvent && document.body.addEventListener(eventName, callback);
      eventList.push({ eventName, callback, isDomEvent });
    }
  };

  const dispatchEvent = (e: CustomEvent) => {
    const events = eventList.filter(event => event.eventName === e.type);
    events.forEach(event => {
      event.callback(e);
    });
  };

  const dispatchToDom = (e: CustomEvent) => {
    document.body.dispatchEvent(e);
  };

  const removeEventListener = (eventName: string, callback: CallbackType) => {
    findEvent({ eventName, callback }).isDomEvent &&
      document.body.removeEventListener(eventName, callback);
    eventList = eventList.filter(
      event => !(event.eventName === eventName && event.callback === callback)
    );
  };

  const removeAllEvents = () => {
    eventList.forEach(event => {
      removeEventListener(event.eventName, event.callback);
    });
    eventList = null;
  };

  return {
    addEventListener,
    removeEventListener,
    removeAllEvents,
    dispatchEvent,
    dispatchToDom,
  };
};

export default eventDomDispatcher;
