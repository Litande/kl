import * as z from "zod";

export const defaultFields = {
  country: {
    name: "",
    offset: null,
  },
  callHours: [],
};

export const schema = z.object({
  callHours: z.array(
    z.object({
      country: z.string().min(1),
      from: z.string().min(1),
      till: z.string().min(1),
    })
  ),
  country: z.object({
    name: z.any(),
    offset: z.any(),
  }),
});
