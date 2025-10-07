const BASE_URL: string | undefined = import.meta.env.VITE_API_URL;

const SAFE_BASE_URL = BASE_URL?.endsWith("/")
  ? BASE_URL.slice(0, BASE_URL.length - 1)
  : BASE_URL;

export const getURL = (url: string): string => {
  console.log(SAFE_BASE_URL + "/" + url);
  return SAFE_BASE_URL + "/" + url;
};
