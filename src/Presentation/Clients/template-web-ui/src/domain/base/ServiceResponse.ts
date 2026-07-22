/**
 * The envelope every API endpoint returns.
 *
 * Only these two fields are on the wire: the server marks StatusCode and
 * IsSuccessful with [JsonIgnore], so success is read from the HTTP status, not
 * from the body. An earlier version of this type declared both, which meant
 * `response.data.isSuccessful` typechecked and was always undefined.
 */
export interface ServiceResponse<T> {
  data: T;
  errors: string[];
}

/** Payload type for endpoints that answer with a status and no body. */
export type NoContent = Record<string, never>;

export default ServiceResponse;
