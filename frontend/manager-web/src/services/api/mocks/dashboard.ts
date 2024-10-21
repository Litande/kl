const dashboardApi = {
  getLeadsStats: () =>
    new Promise(res => {
      res([
        {
          country: "CA",
          amount: 50,
          rate: "52%",
          avgTime: "10:50:11",
        },
        {
          country: "EU",
          amount: 50,
          rate: "52%",
          avgTime: "10:50:11",
        },
      ]);
    }),
};

export default dashboardApi;
