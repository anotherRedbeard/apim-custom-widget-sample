export type Values = {
  identityApiUrl: string
}

export const valuesDefault: Readonly<Values> = Object.freeze({
  identityApiUrl: "http://your-identity-url/api",
})
