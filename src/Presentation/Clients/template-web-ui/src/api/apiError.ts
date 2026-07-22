import axios from "axios";

/**
 * Pulls the first message out of the API's ServiceResponse envelope
 * (`{ errors: string[] }`), falling back to a caller-supplied default.
 *
 * Reaching straight for `error.response.data.errors[0]` throws whenever the
 * request never reached the server - a network failure has no `response`.
 */
export const apiErrorMessage = (error: unknown, fallback: string): string => {
  if (axios.isAxiosError(error)) {
    const errors = error.response?.data?.errors;
    if (Array.isArray(errors) && typeof errors[0] === "string") {
      return errors[0];
    }
  }
  return fallback;
};
