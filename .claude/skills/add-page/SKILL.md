---
name: add-page
description: Add a page to the React client in this template - route, component, feature-scoped API service, i18n keys in both locales, and a verified build. Use when the user asks for a new screen, page, view, or route in the web UI, or wants to wire the frontend to an existing API endpoint.
---

# Add a frontend page

Client lives in `src/Presentation/Clients/template-web-ui`. Read its
`CLAUDE.md` first — the token, error and i18n rules there are the ones that
have already been broken once.

## Before writing anything

Read the existing auth page as the reference:

- `src/pages/auth/Auth.tsx` and `components/Login.tsx`
- `src/pages/auth/services/auth-services.ts`
- `src/api/axiosInstance.ts`, `tokenStorage.ts`, `apiError.ts`

Confirm with the user what the page shows and which endpoints it calls, if not
already clear.

## Files

**1. Page** — `src/pages/<name>/<Name>.tsx`

Function component, typed props, `useTranslation()` for every visible string.
UI uses `semantic-ui-react` and the tokens in `utils/theme.ts` — do not
hardcode colours.

**2. Components** — `src/pages/<name>/components/`

Only for pieces used by this page alone. Anything shared moves up.

**3. API calls** — `src/pages/<name>/services/<name>-services.ts`

```ts
import api from "../../../api/axiosInstance";
import { apiErrorMessage } from "../../../api/apiError";

export const getThings = async (): Promise<Thing[]> => {
  try {
    const response = await api.get<{ data: Thing[] }>("/thing");
    return response.data.data;   // ServiceResponse envelope
  } catch (error) {
    ToastrService.error(apiErrorMessage(error, i18n.t("thingLoadError")));
    return [];
  }
};
```

Always `api` from `axiosInstance` — never bare `axios`, or the request loses
its auth header and base URL. Always `apiErrorMessage` — a network failure has
no `error.response`, so reaching into it throws.

**4. Route** — `src/App.tsx`

```tsx
<Route path="/<name>" element={<Name />} />
```

If the page requires authentication, guard it on
`useSelector((s: RootState) => s.auth.isAuthenticated)` and redirect to
`/auth`.

**5. Shared state** — only if more than one page needs it. Then it is a slice
in `src/features/`, registered in `store.ts`. A single page's state stays in
the component.

**6. i18n** — add every key to **both** `public/locales/en/translation.json`
and `public/locales/tr/translation.json`. A key present in one file renders as
the raw key in the other language.

## Verify

```bash
cd src/Presentation/Clients/template-web-ui
npm run build     # tsc -b && vite build - this is what typechecks
npm run lint
```

`npm run dev` does not typecheck; a change is not verified until `npm run build`
passes.

Then check the keys line up:

```bash
diff <(jq -S 'keys' public/locales/en/translation.json) \
     <(jq -S 'keys' public/locales/tr/translation.json)
```

To see it running, start the API (`dotnet run --project src/Presentation/Template.WebAPI`)
and `npm run dev`, then open the page in the browser and confirm it renders and
its requests succeed. Report what you verified rather than asking the user to
check.
