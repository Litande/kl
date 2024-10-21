import { useEffect, useState } from "react";
import trackingApi from "services/api/tracking";

type Props = {
  withAgents?: boolean;
  withCountries?: boolean;
  withStatuses?: boolean;
};

interface IOption {
  label: string | number;
  value: string | number;
  parentId?: string | number;
}

const cache = {
  countries: [],
  statuses: [],
};

type UseOptions = (props: Props) => {
  statuses: IOption[];
  countries: IOption[];
  agents: IOption[];
};

const useOptions: UseOptions = ({ withAgents, withCountries, withStatuses }) => {
  const [agents, setAgents] = useState([]);
  const [statuses, setStatuses] = useState(cache.statuses);
  const [countries, setCountries] = useState(cache.countries);

  useEffect(() => {
    if (withAgents) {
      trackingApi.getAgents().then(({ data }) => {
        const items = data.items.map(({ id, name }) => ({
          value: id,
          label: name,
        }));
        setAgents(items);
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (withStatuses && !statuses.length) {
      trackingApi.getLeadStatuses().then(({ data }) => {
        setStatuses(data);
        cache.statuses = data;
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (withCountries && !countries.length) {
      trackingApi.getCountries().then(({ data }) => {
        setCountries(data);
        cache.countries = data;
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return { countries, statuses, agents };
};

export default useOptions;
