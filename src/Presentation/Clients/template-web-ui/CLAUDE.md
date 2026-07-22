# template-web-ui

React 18 + TypeScript + Vite SPA for the Template API. The root `CLAUDE.md`
covers the backend; this file covers only what differs here.

## Commands

```bash
npm install
npm run dev      # http://localhost:3000
npm run build    # tsc -b && vite build - THIS is what typechecks
npm run lint
```

`npm run dev` does not typecheck. A change is not verified until `npm run build`
passes.

## Layout

```
src/
  api/          axiosInstance, tokenStorage, apiError - all HTTP plumbing
  domain/       response and token types mirroring the API contract
  features/     Redux Toolkit slices (authSlice)
  pages/        <page>/Page.tsx + components/ + services/
  utils/        i18n, theme, toastr
```

## Rules

**Tokens.** Only `api/tokenStorage.ts` touches `localStorage` for tokens.
Hardcoding a key elsewhere is exactly how the interceptor ended up reading
`"token"` while login wrote `"accessToken"`, leaving every request unauthenticated.

**Errors.** Use `apiErrorMessage(error, fallback)`. `error.response.data.errors[0]`
throws whenever the request never reached the server — a network error has no
`response`, and `error` is `unknown` under strict TS.

**Auth state.** `features/authSlice.ts` owns sign-in/sign-out. Identity comes
from decoding the access token, never from a response body — the login response
carries a token pair, not a user. Don't add a second copy of this logic in a
page service.

**API shape.** Every endpoint returns the `ServiceResponse` envelope, so payload
lives at `response.data.data`, and errors at `response.data.errors[]`.

**i18n.** Every user-facing string is a key, present in **both**
`public/locales/en/translation.json` and `public/locales/tr/translation.json`.
A missing key renders as the raw key.

**Env.** Config comes from `import.meta.env.VITE_*` with a sane fallback. Copy
`.env.example` to `.env.local`. Only `VITE_`-prefixed variables reach the bundle
— and anything that reaches the bundle is public, so no secrets.

## Adding a page

1. `pages/<name>/<Name>.tsx` — the page
2. `pages/<name>/components/` — pieces used only by it
3. `pages/<name>/services/` — its API calls, using `api` from `api/axiosInstance`
4. route in `App.tsx`
5. i18n keys in both locale files
6. `npm run build` to verify
