using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using NorthwindCms.Models;
using System.Linq;

namespace NorthwindCms.Controllers
{
  public class CmsController : Controller
  {
    private readonly IApi _api;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public CmsController(IApi api)
    {
      _api = api;
    }

    /// <summary>
    /// Gets the blog archive with the given id.
    /// </summary>
    /// <param name="id">The unique page id</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional month</param>
    /// <param name="page">The optional page</param>
    /// <param name="category">The optional category</param>
    /// <param name="tag">The optional tag</param>
    [Route("archive")]
    public async Task<IActionResult> Archive(Guid id, int? year = null, int? month = null, int? page = null,
        Guid? category = null, Guid? tag = null)
    {
      var model = await _api.Pages.GetByIdAsync<BlogArchive>(id);
      model.Archive = await _api.Archives.GetByIdAsync(id, page, category, tag, year, month);

      return View(model);
    }

    /// <summary>
    /// Gets the page with the given id.
    /// </summary>
    /// <param name="id">The unique page id</param>
    [Route("page")]
    public async Task<IActionResult> Page(Guid id)
    {
      var model = await _api.Pages.GetByIdAsync<StandardPage>(id);

      return View(model);
    }

    /// <summary>
    /// Gets the page with the given id.
    /// </summary>
    /// <param name="id">The unique page id</param>
    [Route("pagewide")]
    public async Task<IActionResult> PageWide(Guid id)
    {
      var model = await _api.Pages.GetByIdAsync<StandardPage>(id);

      return View(model);
    }

    /// <summary>
    /// Gets the post with the given id.
    /// </summary>
    /// <param name="id">The unique post id</param>
    ///
    [Route("post")]
    public async Task<IActionResult> Post(Guid id)
    {
      var model = await _api.Posts.GetByIdAsync<BlogPost>(id);

      return View(model);
    }

    [Route("catalog")]
    public async Task<IActionResult> Catalog(Guid id)
    {
      var catalog = await _api.Pages.GetByIdAsync<CatalogPage>(id);

      var model = new CatalogViewModel
      {
        CatalogPage = catalog,
        Categories = (await _api.Sites.GetSitemapAsync())

          // get the catalog page
          .Where(item => item.Id == catalog.Id)

          // get its children
          .SelectMany(item => item.Items)
          
          // for the child sitemap item, get the page,
          // and return a simplified model for the view
          .Select(item =>
            {
              var page = _api.Pages.GetByIdAsync<CategoryPage>(item.Id).Result;

              var ci = new CategoryItem
              {
                Title = page.Title,
                Description = page.CategoryDetail.Description,
                PageUrl = page.Permalink,
                ImageUrl = page.CategoryDetail.CategoryImage
                  .Resize(_api, 200)
              };

              return ci;
            })
      };

      return View(model);
    }

    [Route("catalog-category")]
    public async Task<IActionResult> Category(Guid id)
    {
      var model = await _api.Pages.GetByIdAsync<Models.CategoryPage>(id);

      return View(model);
    }

  }
}
