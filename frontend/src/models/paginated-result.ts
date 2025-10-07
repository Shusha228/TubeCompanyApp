interface PaginatedResultMeta {
  totalPages: number;
  page: number;
  pageLimit: number;
  totalCount: number;
}

export interface PaginatedResult<T> {
  items: T[];
  meta: PaginatedResultMeta;
}
