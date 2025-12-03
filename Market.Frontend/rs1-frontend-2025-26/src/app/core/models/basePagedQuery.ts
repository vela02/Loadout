import {PageRequest} from './pageRequest';

// pratimo klasu c# BasePagedQuery.cs
export class BasePagedQuery {
  paging:PageRequest = {
    page :1,
    pageSize : 10
  }
}
